/**
 * analytics.js - полная логика страницы аналитики
 * Исправления:
 * - Пагинация в модальном окне выбора проектов
 * - Экспорт в Excel через SheetJS (без предупреждений)
 * - Сообщения при отсутствии данных / невозможности построить график
 * - Обработка ошибок "хотя бы один элемент" без сброса галочек
 * - Сохранение/изменение шаблонов с предзаполнением названия
 * - Сброс выбора шаблона при сбросе фильтров
 */

document.addEventListener('DOMContentLoaded', async () => {
    if (!window.checkAuth) return;
    window.initHeader();

    // Элементы DOM
    const categorySelect = document.getElementById('categorySelect');
    const directionDropdown = document.getElementById('directionDropdown');
    const departmentDropdown = document.getElementById('departmentDropdown');
    const statusDropdown = document.getElementById('statusDropdown');
    const rankMinInput = document.getElementById('filterRankMin');
    const rankMaxInput = document.getElementById('filterRankMax');
    const applyFiltersBtn = document.getElementById('applyFiltersBtn');
    const resetFiltersBtn = document.getElementById('resetFiltersBtn');
    const selectedCountDiv = document.getElementById('selectedCount');
    const projectsCountSpan = document.getElementById('projectsCount');
    const dataFieldsContainer = document.getElementById('dataFieldsContainer');
    const saveTemplateBtn = document.getElementById('saveAsTemplateBtn');
    const editTemplateBtn = document.getElementById('editTemplateBtn');
    const templateSelect = document.getElementById('templateSelect');
    const templatesListDiv = document.getElementById('templatesList');
    const templateSearch = document.getElementById('templateSearch');
    const exportExcelBtn = document.getElementById('exportExcel');
    const summaryDepartmentDropdown = document.getElementById('summaryDepartmentDropdown');
    const summaryStatusDropdown = document.getElementById('summaryStatusDropdown');
    const summaryDirectionDropdown = document.getElementById('summaryDirectionDropdown');
    const summaryCategoryDropdown = document.getElementById('summaryCategoryDropdown');
    const summaryDateFrom = document.getElementById('summaryDateFrom');
    const summaryDateTo = document.getElementById('summaryDateTo');
    const resetSummaryFiltersBtn = document.getElementById('resetSummaryFiltersBtn');
    const summaryTableBody = document.querySelector('#summaryTable tbody');
    const totalProjectsSpan = document.getElementById('totalProjects');
    const totalBudgetSpan = document.getElementById('totalBudget');
    const summaryChartCanvas = document.getElementById('summaryChart');
    const reportChartCanvas = document.getElementById('reportChart');
    const projectModal = document.getElementById('projectSelectionModal');
    const closeProjectBtn = document.getElementById('closeProjectModal');
    const projectSearch = document.getElementById('projectSearch');
    const projectListDiv = document.getElementById('projectList');
    const saveProjectSelectionBtn = document.getElementById('saveProjectSelection');
    const templateNameModal = document.getElementById('templateNameModal');
    const templateNameInput = document.getElementById('templateNameInput');
    const templateModalTitle = document.getElementById('templateModalTitle');
    const cancelTemplateModal = document.getElementById('cancelTemplateModal');
    const confirmTemplateSave = document.getElementById('confirmTemplateSave');

    // Состояние
    let currentCategoryId = 1;
    let currentExcludedProjectIds = [];
    let currentSelectedFields = ['Бюджет проекта', 'Срок окупаемости', 'Рентабельность', 'Риск проекта', 'NPV', 'IRR'];
    let currentFilteredProjects = [];
    let allAvailableProjects = [];
    let allProjectsDetails = [];
    let templates = [];
    let currentTemplateId = null;
    let reportChart = null;
    let summaryChart = null;
    const userRole = window.getUserRole();
    const isInvestor = (userRole === 'Investor' || userRole === 'Admin');
    const isApplicant = (userRole === 'Applicant');

    // Пагинация в модалке
    let modalCurrentPage = 1;
    let modalTotalPages = 1;
    const MODAL_PAGE_SIZE = 5;

    const FIXED_FIELDS = [
        'Бюджет проекта', 'Срок окупаемости', 'Рентабельность', 'Риск проекта', 'NPV', 'IRR'
    ];

    function toUTCDate(dateStr) {
        if (!dateStr) return null;
        return new Date(dateStr).toISOString();
    }

    // Скрыть сводку для не-инвесторов
    const summaryTabButton = document.querySelector('.tab-button[data-tab="summary"]');
    if (!isInvestor && summaryTabButton) summaryTabButton.style.display = 'none';

    if (isApplicant && departmentDropdown) {
        const parent = departmentDropdown.closest('.filter-group');
        if (parent) parent.style.display = 'none';
    }

    function filterOutDraftStatus(items) {
        return items.filter(s => s.id !== 1);
    }

    // Дропдауны
    function updateDropdownButton(dropdown) {
        const checkboxes = dropdown.querySelectorAll('.dropdown-menu input[type="checkbox"]');
        const checked = Array.from(checkboxes).filter(cb => cb.checked);
        const total = checkboxes.length;
        const trigger = dropdown.querySelector('.dropdown-trigger');
        if (checked.length === total) trigger.textContent = 'Все';
        else if (checked.length === 0) trigger.textContent = 'Ничего не выбрано';
        else trigger.textContent = `Выбрано: ${checked.length}`;
    }

    function getSelectedIds(dropdownId) {
        const container = document.getElementById(dropdownId);
        if (!container) return [];
        const checkboxes = container.querySelectorAll('.dropdown-menu input[type="checkbox"]:checked');
        return Array.from(checkboxes).map(cb => parseInt(cb.value));
    }

    function setDropdownValues(dropdownId, ids) {
        const container = document.getElementById(dropdownId);
        if (!container) return;
        const checkboxes = container.querySelectorAll('.dropdown-menu input[type="checkbox"]');
        checkboxes.forEach(cb => {
            cb.checked = ids.includes(parseInt(cb.value));
        });
        updateDropdownButton(container);
    }

    function fillDropdown(containerId, items) {
        const container = document.getElementById(containerId);
        if (!container) return;
        const menu = container.querySelector('.dropdown-menu');
        menu.innerHTML = '';
        items.forEach(item => {
            const label = document.createElement('label');
            label.className = 'checkbox-label';
            label.innerHTML = `<input type="checkbox" value="${item.id}" checked><span class="checkmark"></span>${item.name}`;
            label.addEventListener('click', (e) => e.stopPropagation());
            const checkbox = label.querySelector('input');
            checkbox.addEventListener('change', (e) => {
                const checkedBoxes = menu.querySelectorAll('input:checked');
                if (checkedBoxes.length === 0) {
                    e.preventDefault();
                    checkbox.checked = true;
                    window.showNotification('Должен быть выбран хотя бы один элемент', 'error');
                    return;
                }
                updateDropdownButton(container);
                // Дополнительно: если это фильтр анализа, то перезагрузить проекты
                if (containerId !== 'summaryDepartmentDropdown' && containerId !== 'summaryStatusDropdown' && 
                    containerId !== 'summaryDirectionDropdown' && containerId !== 'summaryCategoryDropdown') {
                    loadProjects();
                } else if (isInvestor) {
                    loadSummary();
                }
            });
            menu.appendChild(label);
        });
        updateDropdownButton(container);
    }

    function initDropdowns() {
        document.querySelectorAll('.dropdown-checkbox').forEach(dd => {
            const trigger = dd.querySelector('.dropdown-trigger');
            if (!trigger) return;
            trigger.addEventListener('click', (e) => {
                e.stopPropagation();
                const isOpen = dd.classList.contains('open');
                document.querySelectorAll('.dropdown-checkbox.open').forEach(d => d.classList.remove('open'));
                if (!isOpen) dd.classList.add('open');
            });
            const menu = dd.querySelector('.dropdown-menu');
            if (menu) menu.addEventListener('click', (e) => e.stopPropagation());
        });
        document.addEventListener('click', () => {
            document.querySelectorAll('.dropdown-checkbox.open').forEach(d => d.classList.remove('open'));
        });
    }

    // Справочники
    async function loadDictionaries() {
        try {
            const categories = await window.api.getCategories();
            categorySelect.innerHTML = '';
            categories.forEach(cat => {
                const option = document.createElement('option');
                option.value = cat.id;
                option.textContent = cat.name;
                categorySelect.appendChild(option);
            });
            if (currentCategoryId) categorySelect.value = currentCategoryId;

            let directions = await window.api.getDirections();
            let departments = await window.api.getDepartments();
            let statuses = await window.api.getStatuses();
            statuses = filterOutDraftStatus(statuses);

            fillDropdown('directionDropdown', directions);
            fillDropdown('departmentDropdown', departments);
            fillDropdown('statusDropdown', statuses);

            if (isInvestor) {
                fillDropdown('summaryDepartmentDropdown', departments);
                fillDropdown('summaryStatusDropdown', statuses);
                fillDropdown('summaryDirectionDropdown', directions);
                fillDropdown('summaryCategoryDropdown', categories);
            }
        } catch (err) {
            window.showNotification(err.message, 'error');
        }
    }

    // Загрузка проектов для таблицы
    async function loadProjects(silent = false) {
        const filters = {
            categoryId: currentCategoryId,
            directionIds: getSelectedIds('directionDropdown'),
            departmentIds: getSelectedIds('departmentDropdown'),
            statusIds: getSelectedIds('statusDropdown'),
            rankMin: rankMinInput.value ? parseFloat(rankMinInput.value) : null,
            rankMax: rankMaxInput.value ? parseFloat(rankMaxInput.value) : null,
            ProjectIds: currentExcludedProjectIds,
            selectedFields: currentSelectedFields,
        };
        try {
            const data = await window.api.getProjectsAnalytics(filters);
            // ответ может быть как массивом, так и объектом с полем projects – оставляем совместимость
            const items = data.items || data;
            currentFilteredProjects = Array.isArray(items) ? items : [];
            projectsCountSpan.textContent = currentFilteredProjects.length;
            renderTable();
            const activeViewerTab = document.querySelector('.viewer-tab.active');
            if (activeViewerTab && activeViewerTab.dataset.tab === 'chart') updateChart();
            //Инфа о количестве проектов(перекрывает ошибки)
            //if (!silent) window.showNotification(`Загружено проектов: ${currentFilteredProjects.length}`, 'info');
        } catch (err) {
            if (!silent) window.showNotification(err.message, 'error');
            currentFilteredProjects = [];
            renderTable();
        }
    }

    function renderTable() {
        const theadRow = document.querySelector('#reportTable thead tr');
        if (!theadRow) return;
        const headers = ['Название проекта', ...currentSelectedFields];
        theadRow.innerHTML = headers.map(h => `<th>${h}</th>`).join('');
        const tbody = document.querySelector('#reportTable tbody');
        tbody.innerHTML = '';
        if (currentSelectedFields.length === 0) {
            tbody.innerHTML = '<tr><td colspan="100">Для отображения данных выберите показатели в шаге 3</td>' + '</tr>';
            return;
        }
        if (currentFilteredProjects.length === 0) {
            tbody.innerHTML = '<tr><td colspan="100">Нет данных для отображения</td>' + '</tr>';
            return;
        }
        currentFilteredProjects.forEach(proj => {
            const row = document.createElement('tr');
            const nameCell = document.createElement('td');
            nameCell.textContent = proj.name;
            nameCell.style.fontWeight = 'bold';
            row.appendChild(nameCell);
            currentSelectedFields.forEach(field => {
                const value = proj.characteristics?.[field] ?? '';
                const cell = document.createElement('td');
                if (typeof value === 'number') {
                    if (field.toLowerCase().includes('бюджет') || field === 'NPV') cell.textContent = window.formatCurrency(value);
                    else if (field === 'IRR' || field === 'Рентабельность') cell.textContent = value + '%';
                    else cell.textContent = value;
                } else {
                    cell.textContent = value;
                }
                row.appendChild(cell);
            });
            tbody.appendChild(row);
        });
    }

    // График
    function showChartMessage(show, msg = '') {
        const chartContainer = document.querySelector('#chartTab .chart-container');
        const canvas = reportChartCanvas;
        if (!chartContainer) return;
        if (show) {
            canvas.style.display = 'none';
            let msgDiv = document.getElementById('chartMessage');
            if (!msgDiv) {
                msgDiv = document.createElement('div');
                msgDiv.id = 'chartMessage';
                msgDiv.style.textAlign = 'center';
                msgDiv.style.padding = '2rem';
                msgDiv.style.color = '#7F8C8D';
                msgDiv.style.fontSize = '16px';
                chartContainer.appendChild(msgDiv);
            }
            msgDiv.textContent = msg;
            msgDiv.style.display = 'block';
        } else {
            canvas.style.display = 'block';
            const msgDiv = document.getElementById('chartMessage');
            if (msgDiv) msgDiv.style.display = 'none';
        }
    }

    function updateChart() {
        const ctx = reportChartCanvas?.getContext('2d');
        if (!ctx) return;
        if (reportChart) reportChart.destroy();

        const numericFields = currentSelectedFields.filter(f => {
            const firstProj = currentFilteredProjects[0];
            if (!firstProj) return false;
            const val = firstProj.characteristics?.[f];
            return typeof val === 'number';
        });

        if (currentFilteredProjects.length < 2 || numericFields.length === 0) {
            showChartMessage(true, 'График построить невозможно. Для построения графика необходимо выбрать минимум 2 проекта и хотя бы 1 показатель.');
            return;
        }
        if (currentFilteredProjects.length > 7) {
            showChartMessage(true, 'Для корректной работы графика необходимо выбрать не более 7 проектов.');
            return;
        }
        showChartMessage(false);
        
        const projectsToShow = currentFilteredProjects;
        
        if (numericFields.length === 1) {
            const field = numericFields[0];
            const datasets = [{
                label: field,
                data: projectsToShow.map(p => p.characteristics[field] || 0),
                backgroundColor: 'rgba(52, 152, 219, 0.6)',
                borderColor: '#3498DB',
                borderWidth: 1
            }];
            reportChart = new Chart(ctx, {
                type: 'bar',
                data: { labels: projectsToShow.map(p => p.name), datasets },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: { y: { beginAtZero: true, title: { display: true, text: field } } }
                }
            });
        }
        else if (numericFields.length === 2) {
            const field1 = numericFields[0];
            const field2 = numericFields[1];
            const datasets = [
                {
                    label: field1,
                    data: projectsToShow.map(p => p.characteristics[field1] || 0),
                    backgroundColor: 'rgba(52, 152, 219, 0.6)',
                    borderColor: '#3498DB',
                    borderWidth: 1,
                    yAxisID: 'y'
                },
                {
                    label: field2,
                    data: projectsToShow.map(p => p.characteristics[field2] || 0),
                    backgroundColor: 'rgba(231, 76, 60, 0.6)',
                    borderColor: '#E74C3C',
                    borderWidth: 1,
                    yAxisID: 'y1'
                }
            ];
            reportChart = new Chart(ctx, {
                type: 'bar',
                data: { labels: projectsToShow.map(p => p.name), datasets },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        y: { beginAtZero: true, title: { display: true, text: field1 } },
                        y1: { beginAtZero: true, position: 'right', title: { display: true, text: field2 }, grid: { drawOnChartArea: false } }
                    }
                }
            });
        }
        else {
            const maxValues = {};
            numericFields.forEach(f => {
                maxValues[f] = Math.max(...projectsToShow.map(p => p.characteristics[f] || 0));
            });
            const datasets = projectsToShow.map((proj, idx) => ({
                label: proj.name,
                data: numericFields.map(f => ((proj.characteristics[f] || 0) / (maxValues[f] || 1)) * 100),
                borderColor: `hsl(${idx * 60}, 70%, 50%)`,
                backgroundColor: `hsla(${idx * 60}, 70%, 50%, 0.2)`,
                borderWidth: 2,
            }));
            reportChart = new Chart(ctx, {
                type: 'radar',
                data: { labels: numericFields, datasets },
                options: { responsive: true, maintainAspectRatio: false, scales: { r: { beginAtZero: true, max: 100 } } }
            });
        }
    }

    // Модальное окно выбора проектов с пагинацией
    async function openProjectModal(page = 1) {
        const filters = {
            categoryId: currentCategoryId,
            directionIds: getSelectedIds('directionDropdown'),
            departmentIds: getSelectedIds('departmentDropdown'),
            statusIds: getSelectedIds('statusDropdown'),
            rankMin: rankMinInput.value ? parseFloat(rankMinInput.value) : null,
            rankMax: rankMaxInput.value ? parseFloat(rankMaxInput.value) : null,
            excludedProjectIds: [],
            selectedFields: [],
            page: page,
            pageSize: MODAL_PAGE_SIZE
        };
        try {
            const response = await window.api.getProjectsAnalytics(filters);
            // Ожидаем структуру { items, totalPages }
            const items = response.items || [];
            modalTotalPages = response.totalPages || 1;
            modalCurrentPage = page;
            allAvailableProjects = items;
            allProjectsDetails = allAvailableProjects.map(p => ({
                id: p.id,
                name: p.name,
                rank: p.rank,
                direction: p.direction || '—',
                department: p.department || '—',
                status: p.status || '—',
                category: p.category || '—'
            }));
            renderProjectList();
            renderModalPagination();
            projectModal.classList.add('active');
        } catch (err) {
            // Если ошибка "должен быть выбран хотя бы один..." – не показываем окно и не сбрасываем галочки
            window.showNotification(err.message, 'error');
        }
    }

    function renderProjectList() {
        const searchTerm = projectSearch.value.toLowerCase();
        const filtered = allProjectsDetails.filter(p => p.name.toLowerCase().includes(searchTerm));
        let html = '';
        filtered.forEach(p => {
            const isExcluded = currentExcludedProjectIds.includes(p.id);
            html += `
                <div class="project-item">
                    <label class="checkbox-label">
                        <input type="checkbox" class="project-checkbox" value="${p.id}" ${!isExcluded ? 'checked' : ''}>
                        <span class="checkmark"></span><strong>${escapeHtml(p.name)}</strong>
                    </label>
                    <div class="project-info">
                        <span>Ранг: ${p.rank ?? '—'}</span>
                        <span>Направление: ${escapeHtml(p.direction)}</span>
                        <span>Подразделение: ${escapeHtml(p.department)}</span>
                        <span>Статус: ${escapeHtml(p.status)}</span>
                        <span>Категория: ${escapeHtml(p.category)}</span>
                    </div>
                </div>
            `;
        });
        projectListDiv.innerHTML = html || '<p>Нет проектов</p>';
    }

    function renderModalPagination() {
        const existingPagination = document.getElementById('modalPagination');
        if (existingPagination) existingPagination.remove();
        if (modalTotalPages <= 1) return;
        const paginationDiv = document.createElement('div');
        paginationDiv.id = 'modalPagination';
        paginationDiv.style.display = 'flex';
        paginationDiv.style.justifyContent = 'center';
        paginationDiv.style.gap = '10px';
        paginationDiv.style.marginTop = '15px';
        paginationDiv.innerHTML = `
            <button class="btn-secondary" id="modalPrevPage" ${modalCurrentPage <= 1 ? 'disabled' : ''}>← Назад</button>
            <span>Страница ${modalCurrentPage} из ${modalTotalPages}</span>
            <button class="btn-secondary" id="modalNextPage" ${modalCurrentPage >= modalTotalPages ? 'disabled' : ''}>Вперёд →</button>
        `;
        projectListDiv.parentNode.appendChild(paginationDiv);
        const prevBtn = document.getElementById('modalPrevPage');
        const nextBtn = document.getElementById('modalNextPage');
        if (prevBtn) prevBtn.addEventListener('click', () => openProjectModal(modalCurrentPage - 1));
        if (nextBtn) nextBtn.addEventListener('click', () => openProjectModal(modalCurrentPage + 1));
    }

    function saveProjectSelection() {
        const checkboxes = document.querySelectorAll('#projectList .project-checkbox');
        const selectedIds = Array.from(checkboxes).filter(cb => cb.checked).map(cb => parseInt(cb.value));
        // excluded = все проекты на текущей странице, которые не выбраны
        currentExcludedProjectIds = allAvailableProjects.filter(p => !selectedIds.includes(p.id)).map(p => p.id);
        loadProjects();
        projectModal.classList.remove('active');
    }

    // Шаблоны
    function showTemplateModal(isUpdate = false, templateName = '') {
        templateModalTitle.innerText = isUpdate ? 'Изменить шаблон' : 'Сохранить шаблон';
        templateNameInput.value = templateName;
        templateNameModal.classList.add('active');
    }

    async function saveTemplate(isUpdate) {
        const name = templateNameInput.value.trim();
        if (!name) {
            window.showNotification('Введите название шаблона', 'error');
            return;
        }
        const filtersJson = JSON.stringify({
            categoryId: currentCategoryId,
            directionIds: getSelectedIds('directionDropdown'),
            departmentIds: getSelectedIds('departmentDropdown'),
            statusIds: getSelectedIds('statusDropdown'),
            rankMin: rankMinInput.value ? parseFloat(rankMinInput.value) : null,
            rankMax: rankMaxInput.value ? parseFloat(rankMaxInput.value) : null,
            selectedFields: currentSelectedFields,
            excludedProjectIds: currentExcludedProjectIds
        });
        try {
            let newId;
            if (isUpdate && currentTemplateId) {
                await window.api.updateTemplate(currentTemplateId, { name, filtersJson });
                window.showNotification(`Шаблон "${name}" обновлён`, 'success');
                newId = currentTemplateId;
            } else {
                // Предполагаем, что createTemplate возвращает объект { id } или просто id
                const response = await window.api.createTemplate({ name, filtersJson });
                newId = response.id || response; // адаптация под ваш бэк
                window.showNotification(`Шаблон "${name}" сохранён`, 'success');
            }
            await loadTemplates();
            if (newId) {
                templateSelect.value = newId;
                currentTemplateId = newId;
            }
            templateNameModal.classList.remove('active');
            // Прокрутка к шагу 1
            document.querySelector('.builder-step').scrollIntoView({ behavior: 'smooth', block: 'start' });
        } catch (err) {
            window.showNotification(err.message, 'error');
        }
    }

    async function loadTemplates() {
        try {
            const data = await window.api.getTemplates();
            templates = data.templates || [];
            renderTemplatesList();
            templateSelect.innerHTML = '<option value="">-- Выберите шаблон --</option>';
            templates.forEach(t => {
                const option = document.createElement('option');
                option.value = t.id;
                option.textContent = `${t.name} (${window.formatDate(t.updatedAt || t.createdAt)})`;
                templateSelect.appendChild(option);
            });
        } catch (err) {
            window.showNotification(err.message, 'error');
        }
    }

    function renderTemplatesList() {
        const searchTerm = templateSearch.value.toLowerCase();
        const filtered = templates.filter(t => t.name.toLowerCase().includes(searchTerm));
        if (!filtered.length) {
            templatesListDiv.innerHTML = '<p class="text-center">Нет сохранённых шаблонов</p>';
            return;
        }
        let html = '';
        filtered.forEach(t => {
            html += `<div class="template-item" data-id="${t.id}"><div class="template-info"><span class="template-name">${escapeHtml(t.name)}</span><span class="template-date">${window.formatDate(t.updatedAt || t.createdAt)}</span></div><button class="delete-template-btn" title="Удалить шаблон">🗑 Удалить</button></div>`;
        });
        templatesListDiv.innerHTML = html;
        document.querySelectorAll('.template-item').forEach(item => {
            const id = parseInt(item.dataset.id);
            item.addEventListener('click', (e) => {
                if (e.target.classList.contains('delete-template-btn')) return;
                applyTemplateById(id);
            });
            const delBtn = item.querySelector('.delete-template-btn');
            delBtn.addEventListener('click', (e) => { e.stopPropagation(); deleteTemplateById(id); });
        });
    }

    async function applyTemplateById(id) {
        const template = templates.find(t => t.id === id);
        if (!template) return;
        try {
            const filters = JSON.parse(template.filtersJson);
            const analysisTab = document.querySelector('.tab-button[data-tab="analysis"]');
            if (analysisTab && !analysisTab.classList.contains('active')) analysisTab.click();
            if (filters.categoryId) { currentCategoryId = filters.categoryId; categorySelect.value = currentCategoryId; }
            if (filters.directionIds) setDropdownValues('directionDropdown', filters.directionIds);
            if (filters.departmentIds) setDropdownValues('departmentDropdown', filters.departmentIds);
            if (filters.statusIds) setDropdownValues('statusDropdown', filters.statusIds);
            rankMinInput.value = filters.rankMin ?? '';
            rankMaxInput.value = filters.rankMax ?? '';
            if (filters.selectedFields) currentSelectedFields = filters.selectedFields;
            if (filters.excludedProjectIds) currentExcludedProjectIds = filters.excludedProjectIds;
            currentTemplateId = id;
            renderDataFields();
            await loadProjects();
            templateSelect.value = id;
            window.showNotification(`Шаблон "${template.name}" применён`, 'success');
        } catch (err) {
            window.showNotification('Ошибка применения шаблона: ' + err.message, 'error');
        }
    }

    async function deleteTemplateById(id) {
        if (!confirm('Удалить шаблон?')) return;
        try {
            await window.api.deleteTemplate(id);
            await loadTemplates();
            if (currentTemplateId === id) currentTemplateId = null;
            window.showNotification('Шаблон удалён', 'info');
        } catch (err) {
            window.showNotification(err.message, 'error');
        }
    }

    async function onTemplateSelectChange() {
        const selectedId = templateSelect.value;
        if (!selectedId) {
            resetAnalysisFilters();
        } else {
            await applyTemplateById(parseInt(selectedId));
        }
    }

    // Сводка по подразделениям
    async function loadSummary() {
        if (!isInvestor) return;
        const filters = {
            departmentIds: getSelectedIds('summaryDepartmentDropdown'),
            dateFrom: summaryDateFrom.value ? toUTCDate(summaryDateFrom.value) : null,
            dateTo: summaryDateTo.value ? toUTCDate(summaryDateTo.value) : null,
            statusIds: getSelectedIds('summaryStatusDropdown'),
            directionIds: getSelectedIds('summaryDirectionDropdown'),
            categoryIds: getSelectedIds('summaryCategoryDropdown')
        };
        try {
            const data = await window.api.getSummaryByDepartments(filters);
            renderSummaryTable(data.departments);
            totalProjectsSpan.textContent = data.totalProjects;
            totalBudgetSpan.textContent = window.formatCurrency(data.totalBudget);
            renderSummaryChart(data.departments);
        } catch (err) {
            window.showNotification(err.message, 'error');
        }
    }

    function renderSummaryTable(departments) {
        if (!summaryTableBody) return;
        summaryTableBody.innerHTML = '';
        departments.forEach(d => {
            const row = document.createElement('tr');
            row.innerHTML = `<td>${escapeHtml(d.departmentName)}</td><td>${d.projectCount}</td><td>${window.formatCurrency(d.totalBudget)}</td>`;
            summaryTableBody.appendChild(row);
        });
        if (departments.length === 0) summaryTableBody.innerHTML = '<tr><td colspan="3">Нет данных</td></tr>';
    }

    function showSummaryChartMessage(show, msg = '') {
        const chartContainer = document.querySelector('#summaryChartTab .chart-container');
        const canvas = summaryChartCanvas;
        if (!chartContainer) return;
        if (show) {
            canvas.style.display = 'none';
            let msgDiv = document.getElementById('summaryChartMessage');
            if (!msgDiv) {
                msgDiv = document.createElement('div');
                msgDiv.id = 'summaryChartMessage';
                msgDiv.style.textAlign = 'center';
                msgDiv.style.padding = '2rem';
                msgDiv.style.color = '#7F8C8D';
                msgDiv.style.fontSize = '16px';
                chartContainer.appendChild(msgDiv);
            }
            msgDiv.textContent = msg;
            msgDiv.style.display = 'block';
        } else {
            canvas.style.display = 'block';
            const msgDiv = document.getElementById('summaryChartMessage');
            if (msgDiv) msgDiv.style.display = 'none';
        }
    }   

    function renderSummaryChart(departments) {
        if (!summaryChartCanvas) return;
        if (summaryChart) summaryChart.destroy();
        
        const ctx = summaryChartCanvas.getContext('2d');
        
        if (!departments.length) {
            showSummaryChartMessage(true, 'Нет данных для отображения');
            return;
        }
        if (departments.length <= 1) {
            showSummaryChartMessage(true, 'Для построения графика необходимо хотя бы 2 подразделения');
            return;
        }
        showSummaryChartMessage(false);
        
        summaryChart = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: departments.map(d => d.departmentName),
                datasets: [{
                    data: departments.map(d => d.projectCount),
                    backgroundColor: ['#3498DB', '#27AE60', '#F39C12', '#E74C3C', '#9B59B6', '#1ABC9C', '#2C3E50', '#E67E22']
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    tooltip: {
                        callbacks: {
                            title: (tooltipItems) => tooltipItems[0].label,
                            label: (context) => {
                                const dept = departments[context.dataIndex];
                                return `Количество проектов: ${dept.projectCount}, Бюджет: ${window.formatCurrency(dept.totalBudget)}`;
                            }
                        }
                    },
                    datalabels: {
                        display: departments.length <= 5 ? true : false,
                        formatter: (value, context) => {
                            const dept = departments[context.dataIndex];
                            return `${dept.projectCount} пр.`;
                        },
                        color: '#fff',
                        backgroundColor: 'rgba(0,0,0,0.6)',
                        borderRadius: 4,
                        padding: { left: 4, right: 4, top: 2, bottom: 2 },
                        font: { weight: 'bold', size: 12 }
                    }
                }
            }
        });
    }

    // Шаг 3
    function renderDataFields() {
        if (!dataFieldsContainer) return;
        let html = '';
        FIXED_FIELDS.forEach(field => {
            const checked = currentSelectedFields.includes(field) ? 'checked' : '';
            html += `<label class="checkbox-label"><input type="checkbox" name="dataField" value="${field}" ${checked}><span class="checkmark"></span>${field}</label>`;
        });
        dataFieldsContainer.innerHTML = html;
        document.querySelectorAll('input[name="dataField"]').forEach(cb => {
            cb.addEventListener('change', () => {
                currentSelectedFields = Array.from(document.querySelectorAll('input[name="dataField"]:checked')).map(cb => cb.value);
                loadProjects();
            });
        });
    }

    // Сброс фильтров
    function resetAnalysisFilters() {
        const dropdowns = ['directionDropdown', 'departmentDropdown', 'statusDropdown'];
        dropdowns.forEach(id => {
            const container = document.getElementById(id);
            if (container) {
                const checkboxes = container.querySelectorAll('.dropdown-menu input[type="checkbox"]');
                checkboxes.forEach(cb => cb.checked = true);
                updateDropdownButton(container);
            }
        });
        rankMinInput.value = '';
        rankMaxInput.value = '';
        currentExcludedProjectIds = [];
        currentSelectedFields = [...FIXED_FIELDS];
        templateSelect.value = '';
        currentTemplateId = null;
        renderDataFields();
        loadProjects();
        window.showNotification('Фильтры сброшены', 'info');
    }

    function resetSummaryFilters() {
        if (!isInvestor) return;
        const dropdowns = ['summaryDepartmentDropdown', 'summaryStatusDropdown', 'summaryDirectionDropdown', 'summaryCategoryDropdown'];
        dropdowns.forEach(id => {
            const container = document.getElementById(id);
            if (container) {
                const checkboxes = container.querySelectorAll('.dropdown-menu input[type="checkbox"]');
                checkboxes.forEach(cb => cb.checked = true);
                updateDropdownButton(container);
            }
        });
        summaryDateFrom.value = '';
        summaryDateTo.value = '';
        loadSummary();
        window.showNotification('Фильтры сводки сброшены', 'info');
    }

    // Экспорт в Excel (через SheetJS)
    function exportToExcel(tableId, title, filterText) {
        const table = document.getElementById(tableId);
        const cloneTable = table.cloneNode(true);
        const container = document.createElement('div');
        const titleElem = document.createElement('h2');
        titleElem.textContent = title;
        titleElem.style.fontSize = '18pt';
        titleElem.style.fontWeight = 'bold';
        const dateElem = document.createElement('p');
        dateElem.textContent = `Дата экспорта: ${new Date().toLocaleString()}`;
        const filterTitle = document.createElement('p');
        filterTitle.innerHTML = '<strong style="font-size:16pt;">Фильтры:</strong>';
        const filterTextElem = document.createElement('p');
        filterTextElem.innerHTML = filterText.replace(/\n/g, '<br>');
        container.appendChild(titleElem);
        container.appendChild(dateElem);
        container.appendChild(filterTitle);
        container.appendChild(filterTextElem);
        container.appendChild(cloneTable);
        const style = `<style>th { background: #f2f2f2; font-weight: bold; border: 1px solid #ddd; padding: 8px; } td { border: 1px solid #ddd; padding: 8px; } table { border-collapse: collapse; width: 100%; }</style>`;
        const html = `<html><head><meta charset="UTF-8">${style}</head><body>${container.innerHTML}</body></html>`;
        const blob = new Blob([html], { type: 'application/vnd.ms-excel' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `${title}_${new Date().toISOString().slice(0,19).replace(/:/g, '-')}.xls`;
        link.click();
        URL.revokeObjectURL(link.href);
    }

    // Вкладки
    function initTabs() {
        const tabButtons = document.querySelectorAll('.tab-button');
        const tabPanes = document.querySelectorAll('.tab-pane');
        tabButtons.forEach(btn => {
            btn.addEventListener('click', () => {
                const tabId = btn.getAttribute('data-tab');
                tabButtons.forEach(b => b.classList.remove('active'));
                tabPanes.forEach(p => p.classList.remove('active'));
                btn.classList.add('active');
                document.getElementById(tabId).classList.add('active');
                if (tabId === 'summary' && isInvestor) loadSummary();
                if (tabId === 'analysis') {
                    const activeViewerTab = document.querySelector('.viewer-tab.active');
                    if (activeViewerTab && activeViewerTab.dataset.tab === 'chart') updateChart();
                }
            });
        });
        const viewerTabs = document.querySelectorAll('.viewer-tab');
        viewerTabs.forEach(tab => {
            tab.addEventListener('click', () => {
                const tabName = tab.dataset.tab;
                viewerTabs.forEach(t => t.classList.remove('active'));
                document.querySelectorAll('.viewer-pane').forEach(p => p.classList.remove('active'));
                tab.classList.add('active');
                document.getElementById(tabName + 'Tab').classList.add('active');
                if (tabName === 'chart') updateChart();
            });
        });
    }

    // Вспомогательные функции для фильтров
    async function getFilterNamesByIds(ids, type) {
        if (!ids.length) return 'не указано';
        let items = [];
        if (type === 'direction') items = await window.api.getDirections();
        else if (type === 'department') items = await window.api.getDepartments();
        else if (type === 'status') items = await window.api.getStatuses();
        else if (type === 'category') items = await window.api.getCategories();
        const selected = items.filter(i => ids.includes(i.id)).map(i => i.name);
        return selected.length ? selected.join(', ') : 'не указано';
    }

    async function getSummaryFiltersDescription() {
        return {
            departments: await getFilterNamesByIds(getSelectedIds('summaryDepartmentDropdown'), 'department'),
            dateFrom: summaryDateFrom.value || 'не указана',
            dateTo: summaryDateTo.value || 'не указана',
            statuses: await getFilterNamesByIds(getSelectedIds('summaryStatusDropdown'), 'status'),
            directions: await getFilterNamesByIds(getSelectedIds('summaryDirectionDropdown'), 'direction'),
            categories: await getFilterNamesByIds(getSelectedIds('summaryCategoryDropdown'), 'category')
        };
    }

    async function getCurrentFiltersDescription() {
        return {
            category: categorySelect.options[categorySelect.selectedIndex]?.text || 'не указана',
            directions: await getFilterNamesByIds(getSelectedIds('directionDropdown'), 'direction'),
            departments: await getFilterNamesByIds(getSelectedIds('departmentDropdown'), 'department'),
            statuses: await getFilterNamesByIds(getSelectedIds('statusDropdown'), 'status'),
            rankMin: rankMinInput.value || 'не указан',
            rankMax: rankMaxInput.value || 'не указан'
        };
    }

    // Инициализация
    async function init() {
        initDropdowns();
        initTabs();
        await loadDictionaries();
        await loadTemplates();
        currentCategoryId = parseInt(categorySelect.value);
        renderDataFields();
        await loadProjects(true);
        if (isInvestor && document.querySelector('.tab-button.active')?.dataset.tab === 'summary') await loadSummary();

        categorySelect.addEventListener('change', () => {
            currentCategoryId = parseInt(categorySelect.value);
            currentExcludedProjectIds = [];
            loadProjects();
        });
        applyFiltersBtn?.addEventListener('click', () => loadProjects());
        resetFiltersBtn?.addEventListener('click', resetAnalysisFilters);
        resetSummaryFiltersBtn?.addEventListener('click', resetSummaryFilters);
        selectedCountDiv?.addEventListener('click', () => openProjectModal(1));

        if (closeProjectBtn) {
            closeProjectBtn.addEventListener('click', () => projectModal.classList.remove('active'));
            window.addEventListener('click', (e) => { if (e.target === projectModal) projectModal.classList.remove('active'); });
        }
        saveProjectSelectionBtn?.addEventListener('click', saveProjectSelection);
        projectSearch?.addEventListener('input', renderProjectList);

        // Шаблоны
        saveTemplateBtn?.addEventListener('click', () => {
            const selectedId = templateSelect.value;
            if (selectedId && selectedId !== '') {
                window.showNotification('Выбран шаблон, возможно вы хотели его изменить.', 'error');
                return;
            }
            showTemplateModal(false);
        });
        editTemplateBtn?.addEventListener('click', () => {
            const selectedId = templateSelect.value;
            if (!selectedId || selectedId === '') {
                window.showNotification('Для изменения шаблона его необходимо выбрать, возможно вы хотели создать новый.', 'error');
                return;
            }
            const selectedTemplate = templates.find(t => t.id == selectedId);
            if (selectedTemplate) showTemplateModal(true, selectedTemplate.name);
            else window.showNotification('Шаблон не найден', 'error');
        });
        confirmTemplateSave?.addEventListener('click', () => {
            const isUpdate = templateModalTitle.innerText === 'Изменить шаблон';
            saveTemplate(isUpdate);
        });
        cancelTemplateModal?.addEventListener('click', () => templateNameModal.classList.remove('active'));
        templateSelect?.addEventListener('change', onTemplateSelectChange);
        templateSearch?.addEventListener('input', renderTemplatesList);

        // Экспорт
        exportExcelBtn?.addEventListener('click', async () => {
            const filters = await getCurrentFiltersDescription();
            const filterText = `Категория: ${filters.category}; Направления: ${filters.directions}; Подразделения: ${filters.departments}; Статусы: ${filters.statuses}; Ранг: от ${filters.rankMin} до ${filters.rankMax}`;
            exportToExcel('reportTable', 'Анализ_инвестиционных_проектов', filterText);
        });
        document.getElementById('exportSummaryExcel')?.addEventListener('click', async () => {
            const filters = await getSummaryFiltersDescription();
            const filterText = `Подразделения: ${filters.departments}; Дата: от ${filters.dateFrom} до ${filters.dateTo}; Статусы: ${filters.statuses}; Направления: ${filters.directions}; Категории: ${filters.categories}`;
            exportToExcel('summaryTable', 'Сводка_по_подразделениям', filterText);
        });

        // Автоматическое обновление при изменении чекбоксов
        const analysisFilters = ['directionDropdown', 'departmentDropdown', 'statusDropdown'];
        analysisFilters.forEach(id => {
            const container = document.getElementById(id);
            if (container) container.querySelector('.dropdown-menu')?.addEventListener('change', () => loadProjects());
        });
        if (isInvestor) {
            const summaryFilters = ['summaryDepartmentDropdown', 'summaryStatusDropdown', 'summaryDirectionDropdown', 'summaryCategoryDropdown'];
            summaryFilters.forEach(id => {
                const container = document.getElementById(id);
                if (container) container.querySelector('.dropdown-menu')?.addEventListener('change', () => loadSummary());
            });
            summaryDateFrom?.addEventListener('change', loadSummary);
            summaryDateTo?.addEventListener('change', loadSummary);
        }
    }

    init();
});

function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/[&<>]/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;' }[m]));
} 