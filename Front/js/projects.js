document.addEventListener('DOMContentLoaded', function () {
    // Исходные данные проектов
    let projects = [
        {
            id: 1,
            name: 'Развитие IT-инфраструктуры',
            category: 'Инфраструктурный',
            rank: 85,
            direction: 'IT',
            department: 'Департамент развития',
            status: 'Активный',
            date: '2024-01-20'
        },
        {
            id: 2,
            name: 'Строительство цеха №2',
            category: 'Стратегический',
            rank: 78,
            direction: 'Производство',
            department: 'Филиал Восток',
            status: 'Активный',
            date: '2024-01-18'
        },
        {
            id: 3,
            name: 'Закупка оборудования',
            category: 'Инновационный',
            rank: 92,
            direction: 'Логистика',
            department: 'Центральный офис',
            status: 'Завершен',
            date: '2024-01-15'
        },
        {
            id: 4,
            name: 'Разработка мобильного приложения',
            category: 'Инновационный',
            rank: 0,
            direction: 'IT',
            department: 'Центральный офис',
            status: 'Черновик',
            date: '2024-02-01'
        }
    ];

    // Текущий пользователь
    const currentUser = {
        name: 'Иванов А.В.',
        department: 'Центральный офис'
    };

    // ----- Вспомогательные функции -----

    // Обновить текст на кнопке дропдауна в зависимости от выбранных чекбоксов
    function updateDropdownTrigger(dropdown) {
        const checkboxes = dropdown.querySelectorAll('.dropdown-menu input[type="checkbox"]');
        const checked = Array.from(checkboxes).filter(cb => cb.checked);
        const total = checkboxes.length;
        const trigger = dropdown.querySelector('.dropdown-trigger');
        const filterName = dropdown.closest('.filter-group')?.querySelector('span')?.innerText || 'Фильтр';
        if (checked.length === 0) {
            trigger.textContent = `Ничего не выбрано`;
        } else if (checked.length === total) {
            trigger.textContent = `Все ${filterName.toLowerCase()}`;
        } else {
            trigger.textContent = `Выбрано: ${checked.length}`;
        }
    }

    // Инициализация дропдаунов
    function initDropdowns() {
        document.querySelectorAll('.dropdown-checkbox').forEach(dropdown => {
            const trigger = dropdown.querySelector('.dropdown-trigger');
            const menu = dropdown.querySelector('.dropdown-menu');
            
            trigger.addEventListener('click', (e) => {
                e.stopPropagation();
                // Закрыть все другие дропдауны
                document.querySelectorAll('.dropdown-checkbox.open').forEach(d => {
                    if (d !== dropdown) d.classList.remove('open');
                });
                dropdown.classList.toggle('open');
            });

            menu.addEventListener('change', (e) => {
                if (e.target.type === 'checkbox') {
                    e.stopPropagation();
                    updateDropdownTrigger(dropdown);
                    const tabId = dropdown.dataset.tab;
                    applyFiltersAndSort(tabId);
                }
            });

            document.addEventListener('click', (e) => {
                if (!dropdown.contains(e.target)) {
                    dropdown.classList.remove('open');
                }
            });

            updateDropdownTrigger(dropdown);
        });
    }

    // Получить выбранные значения из дропдауна
    function getSelectedFromDropdown(tabId, filterType) {
        const dropdown = document.querySelector(`.dropdown-checkbox[data-filter="${filterType}"][data-tab="${tabId}"]`);
        if (!dropdown) return [];
        const checkboxes = dropdown.querySelectorAll('.dropdown-menu input[type="checkbox"]:checked');
        return Array.from(checkboxes).map(cb => cb.value);
    }

    // Отрисовка конкретной вкладки
    function renderProjects(tabId, projectsToRender) {
        const tbody = document.getElementById(`${tabId}-table-body`);
        if (!tbody) return;
        tbody.innerHTML = '';
        projectsToRender.forEach(proj => {
            const row = document.createElement('tr');
            row.className = 'project-row';
            row.setAttribute('data-project-id', proj.id);
            row.innerHTML = `
                <td class="project-name">${proj.name}</td>
                <td class="project-category">${proj.category}</td>
                <td class="project-rank">${proj.rank}</td>
                <td class="project-direction">${proj.direction}</td>
                <td class="project-department">${proj.department}</td>
                <td class="project-status"><span class="status-badge status-${proj.status === 'Активный' ? 'active' : proj.status === 'Завершен' ? 'completed' : 'draft'}">${proj.status}</span></td>
                <td class="project-date">${proj.date}</td>
            `;
            row.addEventListener('click', function (e) {
                if (e.target.tagName !== 'INPUT' && !e.target.closest('.dropdown-trigger')) {
                    window.location.href = `project-detail.html?id=${proj.id}`;
                }
            });
            tbody.appendChild(row);
        });
    }

    // Применение фильтров и сортировки
    function applyFiltersAndSort(tabId) {
        // Базовый набор проектов для вкладки
        let baseProjects = [];
        if (tabId === 'current') {
            baseProjects = projects.filter(p => p.status === 'Активный');
        } else if (tabId === 'drafts') {
            baseProjects = projects.filter(p => p.status === 'Черновик');
        } else if (tabId === 'archive') {
            baseProjects = projects.filter(p => p.status === 'Завершен');
        }

        let filteredProjects = [...baseProjects];

        // Поиск по названию
        const searchInput = document.querySelector(`.project-search[data-tab="${tabId}"]`);
        const searchTerm = searchInput ? searchInput.value.toLowerCase() : '';
        if (searchTerm) {
            filteredProjects = filteredProjects.filter(p => p.name.toLowerCase().includes(searchTerm));
        }

        // Фильтр по направлению
        const selectedDirections = getSelectedFromDropdown(tabId, 'direction');
        if (selectedDirections.length > 0) {
            filteredProjects = filteredProjects.filter(p => selectedDirections.includes(p.direction));
        }

        // Фильтр по подразделению
        const selectedDepartments = getSelectedFromDropdown(tabId, 'department');
        if (selectedDepartments.length > 0) {
            filteredProjects = filteredProjects.filter(p => selectedDepartments.includes(p.department));
        }

        // Фильтр по категории
        const selectedCategories = getSelectedFromDropdown(tabId, 'category');
        if (selectedCategories.length > 0) {
            filteredProjects = filteredProjects.filter(p => selectedCategories.includes(p.category));
        }

        // Фильтр по статусу (только для вкладки current)
        if (tabId === 'current') {
            const selectedStatuses = getSelectedFromDropdown(tabId, 'status');
            if (selectedStatuses.length > 0) {
                filteredProjects = filteredProjects.filter(p => selectedStatuses.includes(p.status));
            }
        }

        // Сортировка
        const sortSelect = document.querySelector(`.filter-select[data-tab="${tabId}"]`);
        const sortValue = sortSelect ? sortSelect.value : 'rank-desc';
        filteredProjects.sort((a, b) => {
            switch (sortValue) {
                case 'rank-desc': return b.rank - a.rank;
                case 'rank-asc': return a.rank - b.rank;
                case 'name': return a.name.localeCompare(b.name);
                case 'date': return new Date(b.date) - new Date(a.date);
                default: return 0;
            }
        });

        renderProjects(tabId, filteredProjects);
    }

    // Переключение вкладок
    const tabButtons = document.querySelectorAll('.tab-button');
    const tabPanes = document.querySelectorAll('.tab-pane');

    tabButtons.forEach(button => {
        button.addEventListener('click', function () {
            const tabId = this.getAttribute('data-tab');
            tabButtons.forEach(btn => btn.classList.remove('active'));
            tabPanes.forEach(pane => pane.classList.remove('active'));
            this.classList.add('active');
            document.getElementById(tabId).classList.add('active');
            applyFiltersAndSort(tabId);
        });
    });

    // Слушатели для поиска и сортировки
    document.querySelectorAll('.project-search').forEach(input => {
        input.addEventListener('input', function () {
            const tabId = this.getAttribute('data-tab');
            applyFiltersAndSort(tabId);
        });
    });

    document.querySelectorAll('.filter-select').forEach(select => {
        select.addEventListener('change', function () {
            const tabId = this.getAttribute('data-tab');
            applyFiltersAndSort(tabId);
        });
    });

    // Модальное окно создания проекта
    function showCreateProjectModal() {
        const modal = document.createElement('div');
        modal.className = 'modal-overlay';
        modal.innerHTML = `
            <div class="modal">
                <div class="modal-header">
                    <h3>Создание нового проекта</h3>
                    <button class="close-modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="newProjectName">Название проекта</label>
                        <input type="text" id="newProjectName" placeholder="Введите название проекта">
                    </div>
                    <div class="form-group">
                        <label for="newProjectCategory">Категория</label>
                        <select id="newProjectCategory">
                            <option value="">Выберите категорию</option>
                            <option value="Стратегический">Стратегический</option>
                            <option value="Инфраструктурный">Инфраструктурный</option>
                            <option value="Инновационный">Инновационный</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="newProjectDirection">Направление</label>
                        <select id="newProjectDirection">
                            <option value="">Выберите направление</option>
                            <option value="IT">IT</option>
                            <option value="Производство">Производство</option>
                            <option value="Логистика">Логистика</option>
                        </select>
                    </div>
                </div>
                <div class="modal-footer">
                    <button class="btn-secondary cancel-btn">Отмена</button>
                    <button class="btn-primary save-btn">Создать проект</button>
                </div>
            </div>
        `;
        document.body.appendChild(modal);

        const closeBtn = modal.querySelector('.close-modal');
        const cancelBtn = modal.querySelector('.cancel-btn');
        const saveBtn = modal.querySelector('.save-btn');

        function closeModal() {
            modal.style.animation = 'fadeOut 0.3s ease forwards';
            setTimeout(() => modal.remove(), 300);
        }

        closeBtn.addEventListener('click', closeModal);
        cancelBtn.addEventListener('click', closeModal);

        saveBtn.addEventListener('click', function () {
            const name = document.getElementById('newProjectName').value.trim();
            const category = document.getElementById('newProjectCategory').value;
            const direction = document.getElementById('newProjectDirection').value;

            if (!name || !category || !direction) {
                showNotification('Заполните все обязательные поля', 'error');
                return;
            }

            const newProject = {
                id: projects.length + 1,
                name: name,
                category: category,
                rank: 0,
                direction: direction,
                department: currentUser.department,
                status: 'Черновик',
                date: new Date().toISOString().slice(0, 10)
            };

            projects.push(newProject);
            // Обновляем все вкладки
            applyFiltersAndSort('current');
            applyFiltersAndSort('drafts');
            applyFiltersAndSort('archive');
            showNotification(`Проект "${name}" создан и помещён в черновики`, 'success');
            closeModal();

            // Переключаемся на вкладку черновиков
            document.querySelector('.tab-button[data-tab="drafts"]').click();
        });
    }

    // Добавляем стили для анимаций, если их нет
    if (!document.querySelector('#modal-styles')) {
        const style = document.createElement('style');
        style.id = 'modal-styles';
        style.textContent = `
            .modal-overlay {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0,0,0,0.5);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 10000;
                animation: fadeIn 0.3s ease;
            }
            .modal {
                background: white;
                border-radius: var(--border-radius);
                box-shadow: var(--box-shadow);
                width: 90%;
                max-width: 500px;
                max-height: 90vh;
                overflow: auto;
                animation: slideUp 0.3s ease;
            }
            .modal-header {
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 1.5rem;
                border-bottom: 1px solid var(--border-color);
            }
            .modal-header h3 {
                margin: 0;
                color: var(--primary-color);
            }
            .close-modal {
                background: none;
                border: none;
                font-size: 1.5rem;
                cursor: pointer;
                color: var(--dark-gray);
            }
            .modal-body {
                padding: 1.5rem;
            }
            .modal-footer {
                padding: 1.5rem;
                border-top: 1px solid var(--border-color);
                display: flex;
                justify-content: flex-end;
                gap: 1rem;
            }
            @keyframes fadeIn {
                from { opacity: 0; }
                to { opacity: 1; }
            }
            @keyframes slideUp {
                from { transform: translateY(50px); opacity: 0; }
                to { transform: translateY(0); opacity: 1; }
            }
            @keyframes fadeOut {
                from { opacity: 1; }
                to { opacity: 0; }
            }
            @keyframes slideOut {
                from { transform: translateX(0); opacity: 1; }
                to { transform: translateX(100%); opacity: 0; }
            }
        `;
        document.head.appendChild(style);
    }

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

    // Инициализация
    initDropdowns();
    const activeTab = document.querySelector('.tab-pane.active')?.id || 'current';
    applyFiltersAndSort(activeTab);

    document.getElementById('createProjectBtn').addEventListener('click', showCreateProjectModal);

    document.querySelector('.logout-btn').addEventListener('click', function () {
        if (confirm('Вы уверены, что хотите выйти из системы?')) {
            window.location.href = 'index.html';
        }
    });
});