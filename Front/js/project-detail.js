document.addEventListener('DOMContentLoaded', function () {
    // ========== База проектов (имитация данных с сервера) ==========
    const projectsData = [
        {
            id: 1,
            name: 'Развитие IT-инфраструктуры',
            category: 'Инфраструктурный',
            direction: 'IT',
            status: 'Активный',
            department: 'Департамент развития',
            date: '2024-01-20',
            rank: 85,
            teoSummary: 'Проект направлен на модернизацию серверного оборудования и сетей.',
            teoTechnical: 'Необходимость обновления обусловлена ростом нагрузки.',
            teoEconomic: 'Окупаемость 3 года.',
            indicatorValues: {
                'Объем инвестиций': 15000000,
                'Срок реализации': 18,
                'Количество пользователей': 25
            },
            cardGoal: 'Повышение производительности IT-инфраструктуры',
            cardStartDate: '2024-02-01',
            cardEndDate: '2024-12-31',
            cardManager: 'Петров И.С.',
            comments: [
                { id: 1, author: 'Иванов А.В.', date: '2024-02-01 10:30', text: 'Нужно уточнить бюджет.' },
                { id: 2, author: 'Петров И.С.', date: '2024-02-02 14:15', text: 'Бюджет утверждён.' }
            ]
        },
        {
            id: 2,
            name: 'Строительство цеха №2',
            category: 'Стратегический',
            direction: 'Производство',
            status: 'Активный',
            department: 'Филиал Восток',
            date: '2024-01-18',
            rank: 78,
            teoSummary: 'Строительство нового цеха для увеличения производственных мощностей.',
            teoTechnical: 'Проект включает возведение здания и закупку оборудования.',
            teoEconomic: 'Окупаемость 5 лет.',
            indicatorValues: {
                'NPV (чистая приведенная стоимость)': 25000000,
                'IRR (внутренняя норма доходности)': 15,
                'Срок окупаемости': 5,
                'Риск проекта': 2
            },
            cardGoal: 'Увеличение производственных мощностей',
            cardStartDate: '2024-03-01',
            cardEndDate: '2025-12-31',
            cardManager: 'Сидоров В.П.',
            comments: []
        },
        {
            id: 3,
            name: 'Закупка оборудования',
            category: 'Инновационный',
            direction: 'Логистика',
            status: 'Завершен',
            department: 'Центральный офис',
            date: '2024-01-15',
            rank: 92,
            teoSummary: 'Закупка нового оборудования для склада.',
            teoTechnical: 'Внедрение автоматизированной системы хранения.',
            teoEconomic: 'Экономия на операционных расходах.',
            indicatorValues: {
                'Потенциальный рынок': 2,
                'Уровень инновации': 8,
                'Срок вывода на рынок': 5
            },
            cardGoal: 'Автоматизация складских операций',
            cardStartDate: '2024-01-15',
            cardEndDate: '2024-06-30',
            cardManager: 'Иванов А.В.',
            comments: [
                { id: 101, author: 'Сидоров П.П.', date: '2024-02-10 09:20', text: 'Проект успешно завершён.' },
                { id: 102, author: 'Иванов А.В.', date: '2024-02-11 11:45', text: 'Все показатели достигнуты.' }
            ]
        },
        {
            id: 4,
            name: 'Разработка мобильного приложения',
            category: 'Инновационный',
            direction: 'IT',
            status: 'Черновик',
            department: 'Центральный офис',
            date: '2024-02-01',
            rank: 0,
            teoSummary: 'Создание мобильного приложения для клиентов.',
            teoTechnical: 'Использование современных фреймворков.',
            teoEconomic: 'Монетизация через подписку.',
            indicatorValues: {
                'Потенциальный рынок': 1.5,
                'Уровень инновации': 6,
                'Срок вывода на рынок': 8
            },
            cardGoal: 'Запуск мобильного приложения',
            cardStartDate: '2024-02-01',
            cardEndDate: '2024-08-31',
            cardManager: 'Петров И.С.',
            comments: []
        }
    ];

    // ========== Показатели для каждой категории ==========
    const indicatorsByCategory = {
        'Стратегический': [
            { name: 'NPV (чистая приведенная стоимость)', unit: 'руб.', range: [ { min: 0, max: 10000000, score: 1 }, { min: 10000000, max: 50000000, score: 2 }, { min: 50000000, max: Infinity, score: 3 } ] },
            { name: 'IRR (внутренняя норма доходности)', unit: '%', range: [ { min: 0, max: 10, score: 1 }, { min: 10, max: 20, score: 2 }, { min: 20, max: Infinity, score: 3 } ] },
            { name: 'Срок окупаемости', unit: 'лет', range: [ { min: 0, max: 2, score: 3 }, { min: 2, max: 4, score: 2 }, { min: 4, max: Infinity, score: 1 } ] },
            { name: 'Риск проекта', unit: 'уровень', range: [ { min: 0, max: 1, score: 3 }, { min: 1, max: 2, score: 2 }, { min: 2, max: Infinity, score: 1 } ] }
        ],
        'Инфраструктурный': [
            { name: 'Объем инвестиций', unit: 'руб.', range: [ { min: 0, max: 5000000, score: 1 }, { min: 5000000, max: 20000000, score: 2 }, { min: 20000000, max: Infinity, score: 3 } ] },
            { name: 'Срок реализации', unit: 'мес.', range: [ { min: 0, max: 12, score: 3 }, { min: 12, max: 24, score: 2 }, { min: 24, max: Infinity, score: 1 } ] },
            { name: 'Количество пользователей', unit: 'тыс.', range: [ { min: 0, max: 10, score: 1 }, { min: 10, max: 50, score: 2 }, { min: 50, max: Infinity, score: 3 } ] }
        ],
        'Инновационный': [
            { name: 'Потенциальный рынок', unit: 'млрд. руб.', range: [ { min: 0, max: 1, score: 1 }, { min: 1, max: 5, score: 2 }, { min: 5, max: Infinity, score: 3 } ] },
            { name: 'Уровень инновации', unit: 'балл', range: [ { min: 0, max: 3, score: 1 }, { min: 3, max: 7, score: 2 }, { min: 7, max: Infinity, score: 3 } ] },
            { name: 'Срок вывода на рынок', unit: 'мес.', range: [ { min: 0, max: 6, score: 3 }, { min: 6, max: 12, score: 2 }, { min: 12, max: Infinity, score: 1 } ] }
        ]
    };

    // Получаем ID из URL
    const urlParams = new URLSearchParams(window.location.search);
    const projectId = urlParams.get('id') || '1';

    // Находим проект по ID
    const foundProject = projectsData.find(p => p.id == projectId);
    if (!foundProject) {
        alert('Проект не найден');
        window.location.href = 'projects.html';
        return;
    }

    // Текущий проект (объединяем найденные данные с дефолтными значениями)
    let currentProject = {
        id: foundProject.id,
        name: foundProject.name,
        category: foundProject.category,
        direction: foundProject.direction,
        status: foundProject.status,
        rank: foundProject.rank || 0,
        department: foundProject.department,
        date: foundProject.date,
        teoSummary: foundProject.teoSummary || '',
        teoTechnical: foundProject.teoTechnical || '',
        teoEconomic: foundProject.teoEconomic || '',
        indicatorValues: foundProject.indicatorValues || {},
        cardGoal: foundProject.cardGoal || '',
        cardStartDate: foundProject.cardStartDate || '',
        cardEndDate: foundProject.cardEndDate || '',
        cardManager: foundProject.cardManager || '',
        comments: foundProject.comments || []
    };

    // Инициализация значений показателей для категории (если каких-то нет)
    const categoryIndicators = indicatorsByCategory[currentProject.category] || indicatorsByCategory['Инфраструктурный'];
    categoryIndicators.forEach(ind => {
        if (currentProject.indicatorValues[ind.name] === undefined) {
            currentProject.indicatorValues[ind.name] = 0;
        }
    });

    // ========== Элементы страницы ==========
    const projectNameSpan = document.getElementById('projectName');
    const projectRankSpan = document.getElementById('projectRank');
    const projectStatusSpan = document.getElementById('projectStatus');
    const backBtn = document.getElementById('backToListBtn');
    const saveBtn = document.getElementById('saveProjectBtn');
    const publishBtn = document.getElementById('publishProjectBtn');
    const commentsTabBtn = document.getElementById('commentsTabBtn');
    const commentsTab = document.getElementById('comments');
    const cardStatusGroup = document.getElementById('cardStatusGroup');
    const cardStatusSelect = document.getElementById('cardStatus');
    const addCommentBtn = document.getElementById('addCommentBtn');

    // ========== Настройка интерфейса в зависимости от статуса ==========
    function setupByStatus() {
        projectNameSpan.textContent = currentProject.name;
        projectStatusSpan.textContent = currentProject.status;

        const isDraft = currentProject.status === 'Черновик';
        const isArchive = currentProject.status === 'Завершен';
        const isActive = currentProject.status === 'Активный';

        if (isDraft) {
            saveBtn.style.display = 'inline-block';
            publishBtn.style.display = 'inline-block';
            commentsTabBtn.style.display = 'none';
            if (commentsTab.classList.contains('active')) {
                document.querySelector('.tab-button[data-tab="teo"]').click();
            }
            cardStatusGroup.style.display = 'none';
            if (addCommentBtn) addCommentBtn.style.display = 'none';
        } else if (isArchive) {
            saveBtn.style.display = 'none';
            publishBtn.style.display = 'none';
            commentsTabBtn.style.display = 'inline-block';
            cardStatusGroup.style.display = 'block';
            cardStatusSelect.disabled = true;
            if (addCommentBtn) addCommentBtn.style.display = 'none'; // скрываем кнопку добавления

            // Делаем все поля ввода недоступными (кроме кнопок)
            document.querySelectorAll('input, textarea, select').forEach(el => {
                if (!el.closest('.project-actions') && el.id !== 'backToListBtn' && el.id !== 'logoutBtn') {
                    el.disabled = true;
                }
            });
        } else { // Активный
            saveBtn.style.display = 'inline-block';
            publishBtn.style.display = 'none';
            commentsTabBtn.style.display = 'inline-block';
            cardStatusGroup.style.display = 'block';
            cardStatusSelect.disabled = false;
            if (addCommentBtn) addCommentBtn.style.display = 'inline-block';
            document.querySelectorAll('input, textarea, select').forEach(el => {
                if (!el.closest('.project-actions')) {
                    el.disabled = false;
                }
            });
        }
    }

    // ========== Заполнение формы из данных проекта ==========
    function populateForms() {
        document.getElementById('cardName').value = currentProject.name;
        document.getElementById('cardDirection').value = currentProject.direction;
        document.getElementById('cardGoal').value = currentProject.cardGoal;
        document.getElementById('cardStartDate').value = currentProject.cardStartDate;
        document.getElementById('cardEndDate').value = currentProject.cardEndDate;
        document.getElementById('cardManager').value = currentProject.cardManager;
        if (cardStatusSelect) cardStatusSelect.value = currentProject.status;

        document.getElementById('teoSummary').value = currentProject.teoSummary;
        document.getElementById('teoTechnical').value = currentProject.teoTechnical;
        document.getElementById('teoEconomic').value = currentProject.teoEconomic;

        renderIndicatorsTable();
        renderComments();
    }

    // ========== Таблица показателей (с учетом статуса архива) ==========
    function renderIndicatorsTable() {
        const tbody = document.getElementById('indicatorsBody');
        tbody.innerHTML = '';
        categoryIndicators.forEach(ind => {
            const row = document.createElement('tr');
            const value = currentProject.indicatorValues[ind.name] || 0;
            
            const tdName = document.createElement('td');
            tdName.textContent = ind.name;
            
            const tdInput = document.createElement('td');
            const input = document.createElement('input');
            input.type = 'number';
            input.className = 'indicator-value';
            input.dataset.name = ind.name;
            input.value = value;
            input.step = 'any';
            // Если проект завершён, поле только для чтения
            if (currentProject.status === 'Завершен') {
                input.disabled = true;
            }
            tdInput.appendChild(input);
            
            const tdUnit = document.createElement('td');
            tdUnit.textContent = ind.unit;
            
            row.appendChild(tdName);
            row.appendChild(tdInput);
            row.appendChild(tdUnit);
            tbody.appendChild(row);
        });

        // Если не архив, добавляем обработчики изменения
        if (currentProject.status !== 'Завершен') {
            document.querySelectorAll('.indicator-value').forEach(input => {
                input.addEventListener('input', function() {
                    const name = this.dataset.name;
                    currentProject.indicatorValues[name] = parseFloat(this.value) || 0;
                    updateRankingAndRank();
                });
            });
        }
    }

    // ========== Расчёт баллов и ранга ==========
    function calculateScores() {
        const scores = [];
        let total = 0;
        categoryIndicators.forEach(ind => {
            const value = currentProject.indicatorValues[ind.name] || 0;
            let score = 1;
            for (let r of ind.range) {
                if (value >= r.min && (r.max === Infinity || value < r.max)) {
                    score = r.score;
                    break;
                }
            }
            scores.push({ name: ind.name, value, unit: ind.unit, score });
            total += score;
        });
        currentProject.rank = total;
        return { scores, total };
    }

    // ========== Обновление вкладки ранжирования и ранга в шапке ==========
    function updateRankingAndRank() {
        const { scores, total } = calculateScores();
        projectRankSpan.textContent = total;

        const tbody = document.getElementById('rankingScoresBody');
        if (tbody) {
            tbody.innerHTML = '';
            scores.forEach(item => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${item.name}</td>
                    <td>${item.value} ${item.unit}</td>
                    <td class="ranking-score">${item.score}</td>
                `;
                tbody.appendChild(row);
            });
        }

        document.getElementById('totalRank').textContent = total;
        updateRankingChart(scores);
    }

    // ========== График ==========
    let chartInstance = null;
    function updateRankingChart(scores) {
        const ctx = document.getElementById('rankingChart')?.getContext('2d');
        if (!ctx) return;
        if (chartInstance) chartInstance.destroy();

        chartInstance = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: scores.map(s => s.name),
                datasets: [{
                    data: scores.map(s => s.score),
                    backgroundColor: ['#3498DB', '#27AE60', '#F39C12', '#E74C3C', '#9B59B6', '#1ABC9C'],
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom' },
                    tooltip: { callbacks: { label: (ctx) => `${ctx.label}: ${ctx.raw} балла` } }
                },
                cutout: '60%'
            }
        });
    }

    // ========== Комментарии ==========
    function renderComments() {
        const container = document.getElementById('commentsList');
        if (!container) return;
        container.innerHTML = '';
        
        const isArchive = currentProject.status === 'Завершен';
        
        currentProject.comments.forEach(comment => {
            const div = document.createElement('div');
            div.className = 'comment-item';
            div.dataset.id = comment.id;
            
            let actionsHtml = '';
            if (!isArchive) {
                actionsHtml = `
                    <div class="comment-actions">
                        <button class="btn-edit-item" title="Редактировать">✎</button>
                        <button class="btn-delete-item" title="Удалить">🗑</button>
                    </div>
                `;
            }
            
            div.innerHTML = `
                <div class="comment-header">
                    <span class="comment-author">${comment.author}</span>
                    <span class="comment-date">${comment.date}</span>
                </div>
                <div class="comment-text">${comment.text}</div>
                ${actionsHtml}
            `;
            container.appendChild(div);
        });

        // Если не архив, добавляем обработчики на кнопки
        if (!isArchive) {
            document.querySelectorAll('.comment-item .btn-edit-item').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    const commentId = parseInt(btn.closest('.comment-item').dataset.id);
                    editComment(commentId);
                });
            });

            document.querySelectorAll('.comment-item .btn-delete-item').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    const commentId = parseInt(btn.closest('.comment-item').dataset.id);
                    deleteComment(commentId);
                });
            });
        }
    }

    function editComment(id) {
        const comment = currentProject.comments.find(c => c.id === id);
        if (!comment) return;
        document.getElementById('commentText').value = comment.text;
        const modal = document.getElementById('commentModal');
        modal.dataset.editId = id;
        modal.classList.add('active');
    }

    function deleteComment(id) {
        if (!confirm('Удалить комментарий?')) return;
        currentProject.comments = currentProject.comments.filter(c => c.id !== id);
        renderComments();
    }

    // ========== Модальное окно комментария ==========
    const modal = document.getElementById('commentModal');
    const closeModal = modal.querySelector('.close-modal');
    const cancelBtn = modal.querySelector('.cancel-btn');
    const saveCommentBtn = document.getElementById('saveCommentBtn');

    function openCommentModal() {
        document.getElementById('commentText').value = '';
        delete modal.dataset.editId;
        modal.classList.add('active');
    }
    function closeCommentModal() { modal.classList.remove('active'); }

    closeModal.addEventListener('click', closeCommentModal);
    cancelBtn.addEventListener('click', closeCommentModal);
    window.addEventListener('click', (e) => { if (e.target === modal) closeCommentModal(); });

    saveCommentBtn.addEventListener('click', () => {
        const text = document.getElementById('commentText').value.trim();
        if (!text) { alert('Введите текст комментария'); return; }
        const editId = modal.dataset.editId;
        if (editId) {
            const comment = currentProject.comments.find(c => c.id == editId);
            if (comment) comment.text = text;
        } else {
            const newComment = {
                id: Date.now(),
                author: 'Иванов А.В.',
                date: new Date().toLocaleString('ru-RU', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' }),
                text: text
            };
            currentProject.comments.push(newComment);
        }
        renderComments();
        closeCommentModal();
    });

    if (addCommentBtn) {
        addCommentBtn.addEventListener('click', openCommentModal);
    }

    // ========== Сохранение проекта ==========
    function saveProject() {
        currentProject.name = document.getElementById('cardName').value;
        currentProject.direction = document.getElementById('cardDirection').value;
        currentProject.cardGoal = document.getElementById('cardGoal').value;
        currentProject.cardStartDate = document.getElementById('cardStartDate').value;
        currentProject.cardEndDate = document.getElementById('cardEndDate').value;
        currentProject.cardManager = document.getElementById('cardManager').value;
        if (cardStatusSelect && !cardStatusSelect.disabled) {
            currentProject.status = cardStatusSelect.value;
        }
        currentProject.teoSummary = document.getElementById('teoSummary').value;
        currentProject.teoTechnical = document.getElementById('teoTechnical').value;
        currentProject.teoEconomic = document.getElementById('teoEconomic').value;

        document.querySelectorAll('.indicator-value').forEach(input => {
            const name = input.dataset.name;
            currentProject.indicatorValues[name] = parseFloat(input.value) || 0;
        });

        updateRankingAndRank();
        projectStatusSpan.textContent = currentProject.status;

        // Обновляем данные в массиве проектов (имитация сохранения на сервере)
        const index = projectsData.findIndex(p => p.id == currentProject.id);
        if (index !== -1) {
            projectsData[index] = { ...projectsData[index], ...currentProject };
        }

        showNotification('Изменения сохранены', 'success');
    }

    // ========== Публикация черновика ==========
    function publishDraft() {
        const allFilled = Object.values(currentProject.indicatorValues).every(v => v && v > 0);
        if (!allFilled) {
            alert('Не все показатели заполнены. Заполните все величины в разделе ТЭО.');
            return;
        }
        if (!confirm('Вы уверены, что хотите опубликовать проект? Он станет доступен в разделе "Текущие проекты".')) {
            return;
        }
        currentProject.status = 'Активный';
        saveProject();
        window.location.href = `project-detail.html?id=${currentProject.id}`;
    }

    // ========== Обработчики ==========
    backBtn.addEventListener('click', () => window.location.href = 'projects.html');
    saveBtn.addEventListener('click', saveProject);
    publishBtn.addEventListener('click', publishDraft);

    // Переключение вкладок
    const tabButtons = document.querySelectorAll('.tab-button');
    const tabPanes = document.querySelectorAll('.tab-pane');
    tabButtons.forEach(btn => {
        btn.addEventListener('click', function() {
            if (this.style.display === 'none') return;
            const tabId = this.dataset.tab;
            tabButtons.forEach(b => b.classList.remove('active'));
            tabPanes.forEach(p => p.classList.remove('active'));
            this.classList.add('active');
            document.getElementById(tabId).classList.add('active');
            if (tabId === 'ranking') updateRankingAndRank();
        });
    });

    // Уведомления
    function showNotification(message, type) {
        const existing = document.querySelector('.notification');
        if (existing) existing.remove();
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.textContent = message;
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 1rem 1.5rem;
            border-radius: var(--border-radius);
            color: white;
            font-weight: 500;
            z-index: 10000;
            animation: slideIn 0.3s ease;
            background-color: ${type === 'error' ? 'var(--accent-color)' : 'var(--success-color)'};
        `;
        document.body.appendChild(notification);
        setTimeout(() => {
            notification.style.animation = 'slideOut 0.3s ease';
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }

    // Выход
    document.querySelector('.logout-btn').addEventListener('click', function() {
        if (confirm('Вы уверены, что хотите выйти?')) window.location.href = 'index.html';
    });

    // ========== Запуск ==========
    setupByStatus();
    populateForms();
    updateRankingAndRank();
});