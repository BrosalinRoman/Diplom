document.addEventListener('DOMContentLoaded', async () => {
    if (!window.checkAuth()) return;
    window.initHeader();

    const urlParams = new URLSearchParams(window.location.search);
    const projectId = parseInt(urlParams.get('id'));
    if (!projectId) { window.location.href = 'control.html'; return; }

    const userRole = window.getUserRole();
    const isInvestor = (userRole === 'Investor' || userRole === 'Admin');
    const isApplicant = (userRole === 'Applicant' || userRole === 'Admin');

    // Элементы DOM
    const projectTitle = document.querySelector('.project-title h1');
    const budgetSpan = document.querySelector('.project-meta span:first-child');
    const investedSpan = document.getElementById('investedTotal');
    const leftSpan = document.getElementById('leftTotal');
    const spentSpan = document.getElementById('spentTotal');
    const progressSpan = document.getElementById('progressPercent');
    const backBtn = document.getElementById('backToListBtn');
    const addInvestBtn = document.getElementById('addInvestBtn');
    const addCostBtn = document.getElementById('addCostBtn');
    const addWorkBtn = document.getElementById('addWorkBtn');
    const investmentsBody = document.getElementById('investmentsBody');
    const costsBody = document.getElementById('costsBody');
    const workBody = document.getElementById('workDoneBody');
    const progressChartCanvas = document.getElementById('progressChart');
    const totalPlanSpan = document.getElementById('totalPlan');
    const totalFactSpan = document.getElementById('totalFact');
    const maxPlanDateSpan = document.getElementById('maxPlanDate');
    const maxFactDateSpan = document.getElementById('maxFactDate');
    const totalCostsSpan = document.getElementById('totalCosts');

    // Модальные окна
    const investModal = document.getElementById('investModal');
    const costModal = document.getElementById('costModal');
    const workModal = document.getElementById('workModal');
    const cancelInvest = document.getElementById('cancelInvestModal');
    const cancelCost = document.getElementById('cancelCostModal');
    const cancelWork = document.getElementById('cancelWorkModal');
    const saveInvestBtn = document.getElementById('saveInvestBtn');
    const saveCostBtn = document.getElementById('saveCostBtn');
    const saveWorkBtn = document.getElementById('saveWorkBtn');
    const investFact = document.getElementById('investFact');
    const investFactDate = document.getElementById('investFactDate');
    const costAmount = document.getElementById('costAmount');
    const costDescription = document.getElementById('costDescription');
    const costResponsible = document.getElementById('costResponsible');
    const costDate = document.getElementById('costDate');
    const workDescription = document.getElementById('workDescription');
    const workProgress = document.getElementById('workProgress');
    const investModalTitle = document.getElementById('investModalTitle');
    const costModalTitle = document.getElementById('costModalTitle');
    const workModalTitle = document.getElementById('workModalTitle');

    let projectBudget = 0;
    let projectName = '';
    let investments = [];
    let costs = [];
    let reports = [];
    let chart = null;
    let currentEditId = null;

    function openModal(modal) { modal.classList.add('active'); }
    function closeModal(modal) { modal.classList.remove('active'); }
    function toUTCDate(dateStr) {
        if (!dateStr) return null;
        return new Date(dateStr).toISOString();
    }

    // Экспорт
    function exportTableToExcel(tableId, fileName) {
        const table = document.getElementById(tableId);
        const wsData = [];
        wsData.push([fileName]);
        wsData.push([`Дата экспорта: ${new Date().toLocaleString()}`]);
        wsData.push([]);
        const rows = table.querySelectorAll('tr');
        rows.forEach(row => {
            const rowData = [];
            row.querySelectorAll('th, td').forEach(cell => rowData.push(cell.innerText));
            wsData.push(rowData);
        });
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet(wsData);
        XLSX.utils.book_append_sheet(wb, ws, 'Данные');
        XLSX.writeFile(wb, `${fileName}_${new Date().toISOString().slice(0,19).replace(/:/g, '-')}.xlsx`);
    }

    async function loadData() {
        try {
            const info = await window.api.getProjectInfo(projectId);
            projectBudget = info.budget;
            projectName = info.name;
            projectTitle.textContent = `Проект: "${projectName}"`;
            budgetSpan.textContent = `Бюджет: ${window.formatCurrency(projectBudget)} BYN`;
            
            // Теперь получаем инвестиции, затраты и отчёты
            investments = await window.api.getInvestments(projectId);
            costs = await window.api.getCosts(projectId);
            reports = await window.api.getProgressReports(projectId);
            reports.sort((a,b) => new Date(a.reportDate) - new Date(b.reportDate));
            
            // Для прогресса и проинвестированной суммы можно использовать данные из инвестиций и отчётов
            const totalInvested = investments.reduce((sum, i) => sum + (i.actualAmount || 0), 0);
            const lastProgress = reports.length ? reports[reports.length - 1].progressPercentage : 0;
            investedSpan.textContent = window.formatCurrency(totalInvested);
            leftSpan.textContent = window.formatCurrency(projectBudget - totalInvested);
            progressSpan.textContent = lastProgress;
            
            renderInvestments();
            renderCosts();
            renderWork();
            updateChart();
        } catch (err) {
            window.showNotification(err.message, 'error');
        }
    }

    function renderInvestments() {
        const thead = document.querySelector('#investmentsTable thead');
        if (thead) {
            let headerHtml = '<tr><th>План, BYN</th><th>Факт, BYN</th><th>Планируемая дата</th><th>Дата внесения</th>';
            if (isInvestor) headerHtml += '<th>Функционал</th>';
            headerHtml += '</tr>';
            thead.innerHTML = headerHtml;
        }

        investmentsBody.innerHTML = '';
        let totalPlan = 0, totalFact = 0;
        let planDates = [], factDates = [];
        investments.forEach(inv => {
            const row = document.createElement('tr');
            const planAmount = inv.plannedAmount ? window.formatCurrency(inv.plannedAmount) : '-';
            const factAmount = inv.actualAmount ? window.formatCurrency(inv.actualAmount) : '-';
            const planDate = inv.plannedDate ? window.formatDate(inv.plannedDate) : '-';
            const factDate = inv.actualDate ? window.formatDate(inv.actualDate) : '-';
            let actionsHtml = '';
            if (isInvestor) {
                actionsHtml = `<button class="btn-icon btn-edit" data-id="${inv.id}">✎</button>`;
                if (inv.actualAmount) actionsHtml += `<button class="btn-icon btn-delete" data-id="${inv.id}">🗑</button>`;
            }
            row.innerHTML = `
                <td>${planAmount}</td>
                <td>${factAmount}</td>
                <td>${planDate}</td>
                <td>${factDate}</td>
                ${isInvestor ? `<td class="action-buttons">${actionsHtml}</td>` : ''}
            `;
            investmentsBody.appendChild(row);
            totalPlan += inv.plannedAmount || 0;
            totalFact += inv.actualAmount || 0;
            if (inv.plannedDate) planDates.push(new Date(inv.plannedDate));
            if (inv.actualDate) factDates.push(new Date(inv.actualDate));
        });
        const maxPlanDate = planDates.length ? new Date(Math.max(...planDates)).toLocaleDateString('ru-RU') : '-';
        const maxFactDate = factDates.length ? new Date(Math.max(...factDates)).toLocaleDateString('ru-RU') : '-';
        if (totalPlanSpan) totalPlanSpan.textContent = window.formatCurrency(totalPlan);
        if (totalFactSpan) totalFactSpan.textContent = window.formatCurrency(totalFact);
        if (maxPlanDateSpan) maxPlanDateSpan.textContent = maxPlanDate;
        if (maxFactDateSpan) maxFactDateSpan.textContent = maxFactDate;
        if (investedSpan) investedSpan.textContent = window.formatCurrency(totalFact);
        if (leftSpan) leftSpan.textContent = window.formatCurrency(projectBudget - totalFact);

        const tfoot = document.querySelector('#investmentsTable tfoot');
        if (tfoot) {
            let footerHtml = '<tr>';
            footerHtml += `<td id="totalPlan">${window.formatCurrency(totalPlan)}</td>`;
            footerHtml += `<td id="totalFact">${window.formatCurrency(totalFact)}</td>`;
            footerHtml += `<td id="maxPlanDate">${maxPlanDate}</td>`;
            footerHtml += `<td id="maxFactDate">${maxFactDate}</td>`;
            if (isInvestor) footerHtml += '<td></td>';
            footerHtml += '</tr>';
            tfoot.innerHTML = footerHtml;
        }

        document.querySelectorAll('#investmentsBody .btn-edit').forEach(btn => {
            btn.addEventListener('click', () => editInvestment(parseInt(btn.dataset.id)));
        });
        document.querySelectorAll('#investmentsBody .btn-delete').forEach(btn => {
            btn.addEventListener('click', () => deleteInvestment(parseInt(btn.dataset.id)));
        });
    }

    function renderCosts() {
        const thead = document.querySelector('#costsTable thead');
        if (thead) {
            let html = '<tr><th>Сумма, BYN</th><th>Описание</th><th>Ответственный</th><th>Дата</th>';
            if (isApplicant) html += '<th>Функционал</th>';
            html += '</tr>';
            thead.innerHTML = html;
        }
        costsBody.innerHTML = '';
        let total = 0;
        costs.forEach(cost => {
            const row = document.createElement('tr');
            let actionsHtml = '';
            if (isApplicant) actionsHtml = `<button class="btn-icon btn-edit" data-id="${cost.id}">✎</button><button class="btn-icon btn-delete" data-id="${cost.id}">🗑</button>`;
            row.innerHTML = `
                <td>${window.formatCurrency(cost.amount)}</td>
                <td>${escapeHtml(cost.description)}</td>
                <td>${escapeHtml(cost.responsible)}</td>
                <td>${window.formatDate(cost.date)}</td>
                ${isApplicant ? `<td class="action-buttons">${actionsHtml}</td>` : ''}
            `;
            costsBody.appendChild(row);
            total += cost.amount;
        });
        if (totalCostsSpan) totalCostsSpan.textContent = window.formatCurrency(total);
        if (spentSpan) spentSpan.textContent = window.formatCurrency(total);

        document.querySelectorAll('#costsBody .btn-edit').forEach(btn => {
            btn.addEventListener('click', () => editCost(parseInt(btn.dataset.id)));
        });
        document.querySelectorAll('#costsBody .btn-delete').forEach(btn => {
            btn.addEventListener('click', () => deleteCost(parseInt(btn.dataset.id)));
        });
    }

    function renderWork() {
        const thead = document.querySelector('#workDoneTable thead');
        if (thead) {
            let html = '<tr><th>Описание</th><th>Прогресс реализации, %</th><th>Дата и время</th>';
            if (isApplicant) html += '<th>Функционал</th>';
            html += '</tr>';
            thead.innerHTML = html;
        }
        workBody.innerHTML = '';
        reports.forEach(report => {
            const row = document.createElement('tr');
            let actionsHtml = '';
            if (isApplicant) actionsHtml = `<button class="btn-icon btn-edit" data-id="${report.id}">✎</button><button class="btn-icon btn-delete" data-id="${report.id}">🗑</button>`;
            row.innerHTML = `
                <td>${escapeHtml(report.description)}</td>
                <td>${report.progressPercentage}%</td>
                <td>${window.formatDate(report.reportDate)}</td>
                ${isApplicant ? `<td class="action-buttons">${actionsHtml}</td>` : ''}
            `;
            workBody.appendChild(row);
        });
        document.querySelectorAll('#workDoneBody .btn-edit').forEach(btn => {
            btn.addEventListener('click', () => editWork(parseInt(btn.dataset.id)));
        });
        document.querySelectorAll('#workDoneBody .btn-delete').forEach(btn => {
            btn.addEventListener('click', () => deleteWork(parseInt(btn.dataset.id)));
        });
    }

    async function deleteInvestment(id) {
        if (!confirm('Удалить инвестицию?')) return;
        try { await window.api.deleteInvestment(id); await loadData(); } catch (err) { window.showNotification(err.message, 'error'); }
    }
    async function editInvestment(id) { openInvestModal(id); }

    async function deleteCost(id) {
        if (!confirm('Удалить затрату?')) return;
        try { await window.api.deleteCost(id); await loadData(); } catch (err) { window.showNotification(err.message, 'error'); }
    }
    async function editCost(id) { openCostModal(id); }

    async function deleteWork(id) {
        if (!confirm('Удалить отчёт?')) return;
        try { await window.api.deleteProgressReport(id); await loadData(); } catch (err) { window.showNotification(err.message, 'error'); }
    }
    async function editWork(id) { openWorkModal(id); }

    // Инвестиции модалка
    function openInvestModal(editId = null) {
        currentEditId = editId;
        if (editId) {
            const inv = investments.find(i => i.id === editId);
            if (inv) {
                investModalTitle.innerText = 'Редактировать инвестицию';
                investFact.value = inv.actualAmount || '';
                investFactDate.value = inv.actualDate ? inv.actualDate.split('T')[0] : '';
            }
        } else {
            investModalTitle.innerText = 'Добавить инвестицию';
            investFact.value = '';
            investFactDate.value = '';
        }
        openModal(investModal);
    }

    async function saveInvestment() {
        const amount = parseFloat(investFact.value);
        const date = investFactDate.value;
        if (isNaN(amount) || amount <= 0 || !date) { window.showNotification('Заполните сумму и дату', 'error'); return; }
        try {
            if (currentEditId) {
                const inv = investments.find(i => i.id === currentEditId);
                await window.api.updateInvestment(currentEditId, {
                    actualAmount: amount,
                    actualDate: toUTCDate(date),
                    plannedAmount: inv.plannedAmount,
                    plannedDate: inv.plannedDate ? toUTCDate(inv.plannedDate.split('T')[0]) : null
                });
            } else {
                await window.api.addInvestment(projectId, { actualAmount: amount, actualDate: toUTCDate(date) });
            }
            await loadData();
            closeModal(investModal);
        } catch (err) { window.showNotification(err.message, 'error'); }
    }

    // Затраты модалка
    function openCostModal(editId = null) {
        currentEditId = editId;
        if (editId) {
            const cost = costs.find(c => c.id === editId);
            if (cost) {
                costModalTitle.innerText = 'Редактировать затраты';
                costAmount.value = cost.amount;
                costDescription.value = cost.description;
                costResponsible.value = cost.responsible;
                costDate.value = cost.date.split('T')[0];
            }
        } else {
            costModalTitle.innerText = 'Добавить затраты';
            costAmount.value = ''; costDescription.value = ''; costResponsible.value = ''; costDate.value = '';
        }
        openModal(costModal);
    }

    async function saveCost() {
        const amount = parseFloat(costAmount.value);
        const description = costDescription.value.trim();
        const responsible = costResponsible.value.trim();
        const date = costDate.value;
        if (!amount || amount <= 0 || !description || !responsible || !date) { window.showNotification('Заполните все поля', 'error'); return; }
        try {
            if (currentEditId) {
                await window.api.updateCost(currentEditId, { amount, description, responsible, date: toUTCDate(date) });
            } else {
                await window.api.addCost(projectId, { amount, description, responsible, date: toUTCDate(date) });
            }
            await loadData();
            closeModal(costModal);
        } catch (err) { window.showNotification(err.message, 'error'); }
    }

    // Отчёты модалка
    function openWorkModal(editId = null) {
        currentEditId = editId;
        if (editId) {
            const report = reports.find(r => r.id === editId);
            if (report) {
                workModalTitle.innerText = 'Редактировать запись';
                workDescription.value = report.description;
                workProgress.value = report.progressPercentage;
            }
        } else {
            workModalTitle.innerText = 'Добавить запись';
            workDescription.value = ''; workProgress.value = '';
        }
        openModal(workModal);
    }

    async function saveWork() {
        const description = workDescription.value.trim();
        const progress = parseFloat(workProgress.value);
        if (!description || isNaN(progress) || progress < 0 || progress > 100) { window.showNotification('Заполните описание и прогресс (0-100)', 'error'); return; }
        try {
            if (currentEditId) {
                await window.api.updateProgressReport(currentEditId, { description, progressPercentage: progress });
            } else {
                await window.api.addProgressReport(projectId, { description, progressPercentage: progress });
            }
            await loadData();
            closeModal(workModal);
        } catch (err) { window.showNotification(err.message, 'error'); }
    }

    // График
    function updateChart() {
        if (!progressChartCanvas) return;
        if (chart) chart.destroy();
        const investPoints = investments.filter(i => i.actualDate && i.actualAmount > 0)
            .map(i => ({ date: new Date(i.actualDate), amount: i.actualAmount }))
            .sort((a,b) => a.date - b.date);
        let cumulative = 0;
        const investPercent = investPoints.map(p => {
            cumulative += p.amount;
            return { date: p.date, percent: (cumulative / projectBudget) * 100 };
        });
        let startDate = investPoints.length ? investPoints[0].date : new Date();
        investPercent.unshift({ date: startDate, percent: 0 });
        const workPoints = reports.map(r => ({ date: new Date(r.reportDate), progress: r.progressPercentage }))
            .sort((a,b) => a.date - b.date);
        workPoints.unshift({ date: startDate, progress: 0 });
        const allDatesSet = new Set();
        investPercent.forEach(p => allDatesSet.add(p.date.toISOString().split('T')[0]));
        workPoints.forEach(p => allDatesSet.add(p.date.toISOString().split('T')[0]));
        const sortedDates = Array.from(allDatesSet).sort((a,b) => new Date(a) - new Date(b));
        const labels = sortedDates.map(d => window.formatDate(d));
        const investData = sortedDates.map(d => {
            const p = investPercent.find(p => p.date.toISOString().split('T')[0] === d);
            return p ? p.percent : null;
        });
        const workData = sortedDates.map(d => {
            const p = workPoints.find(p => p.date.toISOString().split('T')[0] === d);
            return p ? p.progress : null;
        });
        const ctx = progressChartCanvas.getContext('2d');
        chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels,
                datasets: [
                    { label: 'Инвестиции (% от бюджета)', data: investData, borderColor: '#3498db', fill: false, tension: 0.3 },
                    { label: 'Прогресс работы (%)', data: workData, borderColor: '#e74c3c', fill: false, tension: 0.3 }
                ]
            },
            options: { responsive: true, maintainAspectRatio: false, scales: { y: { beginAtZero: true, max: 100 } } }
        });
    }

    // Обработчики
    backBtn?.addEventListener('click', () => window.location.href = 'control.html');
    addInvestBtn?.addEventListener('click', () => openInvestModal());
    addCostBtn?.addEventListener('click', () => openCostModal());
    addWorkBtn?.addEventListener('click', () => openWorkModal());
    cancelInvest?.addEventListener('click', () => closeModal(investModal));
    cancelCost?.addEventListener('click', () => closeModal(costModal));
    cancelWork?.addEventListener('click', () => closeModal(workModal));
    saveInvestBtn?.addEventListener('click', saveInvestment);
    saveCostBtn?.addEventListener('click', saveCost);
    saveWorkBtn?.addEventListener('click', saveWork);
    window.addEventListener('click', (e) => {
        if (e.target === investModal) closeModal(investModal);
        if (e.target === costModal) closeModal(costModal);
        if (e.target === workModal) closeModal(workModal);
    });

    // Кнопки экспорта
    document.querySelectorAll('.export-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            const tableId = btn.dataset.table;
            let title = btn.dataset.title;
            if (title && projectName) title += ` "${projectName}"`;
            exportTableToExcel(tableId, title);
        });
    });

    // Вкладки
    const tabButtons = document.querySelectorAll('.tab-button');
    const tabPanes = document.querySelectorAll('.tab-pane');
    tabButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const tabId = btn.getAttribute('data-tab');
            tabButtons.forEach(b => b.classList.remove('active'));
            tabPanes.forEach(p => p.classList.remove('active'));
            btn.classList.add('active');
            document.getElementById(tabId).classList.add('active');
            if (tabId === 'visual') updateChart();
        });
    });

    if (!isInvestor && addInvestBtn) addInvestBtn.style.display = 'none';
    if (!isApplicant) { if (addCostBtn) addCostBtn.style.display = 'none'; if (addWorkBtn) addWorkBtn.style.display = 'none'; }

    await loadData();
});

function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/[&<>]/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;' }[m]));
}