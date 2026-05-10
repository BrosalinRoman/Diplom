document.addEventListener('DOMContentLoaded', function () {
    // Обработка выхода из системы
    const logoutBtn = document.querySelector('.logout-btn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function () {
            if (confirm('Вы уверены, что хотите выйти из системы?')) {
                window.location.href = 'index.html';
            }
        });
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
        });
    });

    // ========== Сворачивание/разворачивание элементов ==========
    // Функция инициализации: все свёрнуты
    function initCollapse() {
        document.querySelectorAll('.list-item .item-content').forEach(content => {
            content.style.display = 'none';
        });
    }

    // Обработчик клика по заголовку (исключая кнопки действий)
    document.addEventListener('click', function (e) {
        const header = e.target.closest('.item-header');
        if (!header) return;
        if (e.target.closest('.item-actions')) return;

        const item = header.closest('.list-item');
        if (!item) return;

        const content = item.querySelector('.item-content');
        if (content) {
            if (content.style.display === 'block') {
                content.style.display = 'none';
                item.classList.remove('open');
            } else {
                content.style.display = 'block';
                item.classList.add('open');
            }
        }
    });

    // ========== Поиск по названию ==========
    const searchInputs = document.querySelectorAll('.search-input');

    searchInputs.forEach(input => {
        input.addEventListener('input', function () {
            const tabId = this.dataset.tab; // formuls, directions, ...
            const searchTerm = this.value.toLowerCase().trim();
            const container = document.getElementById(`${tabId}-list`);
            if (!container) return;

            const items = container.querySelectorAll('.list-item');
            items.forEach(item => {
                const title = item.querySelector('h4')?.textContent.toLowerCase() || '';
                if (title.includes(searchTerm)) {
                    item.style.display = '';
                } else {
                    item.style.display = 'none';
                }
            });
        });
    });

    // ========== Модальные окна ==========
    function showItemModal(type, itemData = null) {
        const modal = document.createElement('div');
        modal.className = 'modal-overlay';

        let fieldsHtml = '';
        if (type === 'formula') {
            fieldsHtml = `
                <div class="form-group">
                    <label for="itemName">Название формулы</label>
                    <input type="text" id="itemName" value="${itemData?.name || ''}" placeholder="Введите название">
                </div>
                <div class="form-group">
                    <label for="formulaExpression">Формула</label>
                    <textarea id="formulaExpression" rows="3" placeholder="Введите выражение">${itemData?.expression || ''}</textarea>
                </div>
                <div class="form-group">
                    <label for="itemDescription">Описание</label>
                    <textarea id="itemDescription" rows="3" placeholder="Опишите формулу">${itemData?.description || ''}</textarea>
                </div>
            `;
        } else if (type === 'direction' || type === 'status' || type === 'division' || type === 'category') {
            fieldsHtml = `
                <div class="form-group">
                    <label for="itemName">Название</label>
                    <input type="text" id="itemName" value="${itemData?.name || ''}" placeholder="Введите название">
                </div>
                <div class="form-group">
                    <label for="itemDescription">Описание</label>
                    <textarea id="itemDescription" rows="3" placeholder="Введите описание">${itemData?.description || ''}</textarea>
                </div>
            `;
        } else if (type === 'range') {
            fieldsHtml = `
                <div class="form-group">
                    <label for="itemName">Название характеристики</label>
                    <input type="text" id="itemName" value="${itemData?.name || ''}" placeholder="Например: Бюджет">
                </div>
                <div class="form-group">
                    <label>Диапазоны и баллы</label>
                    <div id="ranges-container">
                        <div class="range-row">
                            <input type="number" class="range-min" placeholder="Мин" step="any">
                            <input type="number" class="range-max" placeholder="Макс" step="any">
                            <input type="number" class="range-score" placeholder="Балл" step="1" min="1">
                        </div>
                    </div>
                    <button type="button" class="btn-secondary" id="add-range-row">+ Добавить диапазон</button>
                </div>
            `;
        } else if (type === 'user') {
            fieldsHtml = `
                <div class="form-group">
                    <label for="userFullName">ФИО</label>
                    <input type="text" id="userFullName" value="${itemData?.fullName || ''}" placeholder="Иванов Иван Иванович">
                </div>
                <div class="form-group">
                    <label for="userLogin">Логин</label>
                    <input type="text" id="userLogin" value="${itemData?.login || ''}" placeholder="Логин">
                </div>
                <div class="form-group">
                    <label for="userEmail">Email</label>
                    <input type="email" id="userEmail" value="${itemData?.email || ''}" placeholder="email@example.com">
                </div>
                <div class="form-group">
                    <label for="userPassword">Пароль</label>
                    <input type="password" id="userPassword" value="${itemData?.password || ''}" placeholder="Пароль">
                </div>
                <div class="form-group">
                    <label for="userDepartment">Подразделение</label>
                    <select id="userDepartment">
                        <option value="НОД-1" ${itemData?.department === 'НОД-1' ? 'selected' : ''}>НОД-1 (Минское отделение)</option>
                        <option value="НОД-2" ${itemData?.department === 'НОД-2' ? 'selected' : ''}>НОД-2 (Барановичское отделение)</option>
                        <option value="НОД-3" ${itemData?.department === 'НОД-3' ? 'selected' : ''}>НОД-3 (Брестское отделение)</option>
                        <option value="НОД-4" ${itemData?.department === 'НОД-4' ? 'selected' : ''}>НОД-4 (Гомельское отделение)</option>
                        <option value="НОД-5" ${itemData?.department === 'НОД-5' ? 'selected' : ''}>НОД-5 (Могилёвское отделение)</option>
                        <option value="НОД-6" ${itemData?.department === 'НОД-6' ? 'selected' : ''}>НОД-6 (Витебское отделение)</option>
                    </select>
                </div>
                <div class="form-group">
                    <label for="userRole">Роль</label>
                    <select id="userRole">
                        <option value="applicant" ${itemData?.role === 'applicant' ? 'selected' : ''}>Заявитель</option>
                        <option value="investor" ${itemData?.role === 'investor' ? 'selected' : ''}>Инвестор</option>
                        <option value="admin" ${itemData?.role === 'admin' ? 'selected' : ''}>Администратор</option>
                    </select>
                </div>
            `;
        }

        modal.innerHTML = `
            <div class="modal">
                <div class="modal-header">
                    <h3>${itemData ? 'Редактирование' : 'Добавление'} ${getTypeName(type)}</h3>
                    <button class="close-modal">&times;</button>
                </div>
                <div class="modal-body">
                    ${fieldsHtml}
                </div>
                <div class="modal-footer">
                    <button class="btn-secondary cancel-btn">Отмена</button>
                    <button class="btn-primary save-btn">Сохранить</button>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        // Обработчики для диапазонов (если это range)
        if (type === 'range') {
            const addRangeBtn = modal.querySelector('#add-range-row');
            const container = modal.querySelector('#ranges-container');

            addRangeBtn.addEventListener('click', function () {
                const newRow = document.createElement('div');
                newRow.className = 'range-row';
                newRow.innerHTML = `
                    <input type="number" class="range-min" placeholder="Мин" step="any">
                    <input type="number" class="range-max" placeholder="Макс" step="any">
                    <input type="number" class="range-score" placeholder="Балл" step="1" min="1">
                    <button type="button" class="btn-delete-row" title="Удалить">✖</button>
                `;
                container.appendChild(newRow);
                newRow.querySelector('.btn-delete-row').addEventListener('click', function () {
                    newRow.remove();
                });
            });
        }

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
            // Здесь логика сохранения (можно собрать данные)
            showNotification(`${itemData ? 'Обновлено' : 'Добавлено'}`, 'success');
            closeModal();
        });
    }

    function getTypeName(type) {
        const names = {
            formula: 'формулы',
            direction: 'направления',
            category: 'категории',
            status: 'статуса',
            range: 'диапазона',
            division: 'подразделения',
            user: 'пользователя'
        };
        return names[type] || 'элемента';
    }

    // Кнопки добавления
    document.querySelectorAll('.add-item-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const type = this.getAttribute('data-type');
            showItemModal(type);
        });
    });

    // Делегирование для кнопок редактирования, удаления, блокировки
    document.addEventListener('click', function (e) {
        // Редактирование
        if (e.target.closest('.btn-edit-item')) {
            const btn = e.target.closest('.btn-edit-item');
            const item = btn.closest('.list-item');
            if (!item) return;

            const activePane = document.querySelector('.tab-pane.active');
            const type = mapTabIdToType(activePane?.id);
            const name = item.querySelector('h4')?.textContent || '';
            showItemModal(type, { name: name });
        }

        // Удаление
        if (e.target.closest('.btn-delete-item')) {
            const btn = e.target.closest('.btn-delete-item');
            const item = btn.closest('.list-item');
            if (!item) return;
            const name = item.querySelector('h4')?.textContent || 'элемент';
            if (confirm(`Удалить "${name}"?`)) {
                item.style.animation = 'slideOut 0.3s ease forwards';
                setTimeout(() => item.remove(), 300);
                showNotification(`Элемент удалён`, 'success');
            }
        }

        // Блокировка/разблокировка пользователя
        if (e.target.closest('.btn-lock-item')) {
            const btn = e.target.closest('.btn-lock-item');
            const item = btn.closest('.list-item');
            if (!item) return;

            item.classList.toggle('locked');
            if (item.classList.contains('locked')) {
                btn.textContent = '🔓';
                btn.title = 'Разблокировать';
            } else {
                btn.textContent = '🔒';
                btn.title = 'Заблокировать';
            }
            const userName = item.querySelector('h4')?.textContent || 'Пользователь';
            const action = item.classList.contains('locked') ? 'заблокирован' : 'разблокирован';
            showNotification(`${userName} ${action}`, 'info');
        }
    });

    function mapTabIdToType(tabId) {
        const map = {
            'formuls': 'formula',
            'directions': 'direction',
            'categories': 'category',
            'statuses': 'status',
            'ranges': 'range',
            'divisions': 'division',
            'users': 'user'
        };
        return map[tabId] || 'formula';
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
            background-color: ${type === 'error' ? 'var(--accent-color)' : type === 'success' ? 'var(--success-color)' : 'var(--secondary-color)'};
        `;
        document.body.appendChild(notification);

        setTimeout(() => {
            notification.style.animation = 'slideOut 0.3s ease';
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }

    // Инициализация сворачивания
    initCollapse();
});