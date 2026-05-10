document.addEventListener('DOMContentLoaded', async () => {
    if (!window.checkAuth()) return;
    window.initHeader();

    // Элементы DOM
    const searchInput = document.querySelector('.project-search');
    const directionDropdown = document.querySelector('.dropdown-checkbox[data-filter="direction"]');
    const departmentDropdown = document.querySelector('.dropdown-checkbox[data-filter="department"]');
    const categoryDropdown = document.querySelector('.dropdown-checkbox[data-filter="category"]');
    const statusDropdown = document.querySelector('.dropdown-checkbox[data-filter="status"]');
    const sortSelect = document.getElementById('sortSelect');
    const dateFrom = document.getElementById('controlDateFrom');
    const dateTo = document.getElementById('controlDateTo');
    const tbody = document.getElementById('control-table-body');
    const paginationDiv = document.getElementById('controlPagination');
    
    let currentPage = 1;
    let totalPages = 1;
    const pageSize = 10;

    async function loadFilters() {
        const directions = await window.api.getDirections();
        const departments = await window.api.getDepartments();
        const categories = await window.api.getCategories();
        const statuses = await window.api.getStatuses();
        // Только Активен и Завершен
        const allowedStatuses = statuses.filter(s => s.name === 'Активен' || s.name === 'Завершен');
        fillDropdown(directionDropdown, directions);
        fillDropdown(departmentDropdown, departments);
        fillDropdown(categoryDropdown, categories);
        fillDropdown(statusDropdown, allowedStatuses);
    }

    function fillDropdown(container, items) {
        if (!container) return;
        const menu = container.querySelector('.dropdown-menu');
        menu.innerHTML = '';
        items.forEach(item => {
            const label = document.createElement('label');
            label.className = 'checkbox-label';
            label.innerHTML = `<input type="checkbox" value="${item.id}" checked><span class="checkmark"></span>${item.name}`;
            label.addEventListener('click', (e) => e.stopPropagation());
            const checkbox = label.querySelector('input');
            // Защита от снятия последнего чекбокса
            checkbox.addEventListener('change', (e) => {
                const checkedBoxes = menu.querySelectorAll('input:checked');
                if (checkedBoxes.length === 0) {
                    e.preventDefault();
                    checkbox.checked = true;
                    window.showNotification('Должен быть выбран хотя бы один элемент', 'error');
                    return;
                }
                updateDropdownButton(container);
                loadProjects(1);
            });
            menu.appendChild(label);
        });
        updateDropdownButton(container);
    }

    function updateDropdownButton(dropdown) {
        const checkboxes = dropdown.querySelectorAll('.dropdown-menu input:checked');
        const total = dropdown.querySelectorAll('.dropdown-menu input').length;
        const trigger = dropdown.querySelector('.dropdown-trigger');
        if (checkboxes.length === total) trigger.textContent = 'Все';
        else if (checkboxes.length === 0) trigger.textContent = 'Ничего не выбрано';
        else trigger.textContent = `Выбрано: ${checkboxes.length}`;
    }

    function getSelectedIds(dropdown) {
        if (!dropdown) return [];
        const checkboxes = dropdown.querySelectorAll('.dropdown-menu input:checked');
        return Array.from(checkboxes).map(cb => parseInt(cb.value));
    }

    async function loadProjects(page = 1) {
        const filters = {
            search: searchInput.value,
            directionIds: getSelectedIds(directionDropdown),
            departmentIds: getSelectedIds(departmentDropdown),
            categoryIds: getSelectedIds(categoryDropdown),
            statusIds: getSelectedIds(statusDropdown),
            dateFrom: dateFrom?.value ? new Date(dateFrom.value).toISOString() : null,
            dateTo: dateTo?.value ? new Date(dateTo.value).toISOString() : null,
            sort: sortSelect.value,
            page: page,
            pageSize: pageSize
        };
        try {
            const response = await window.api.getControlProjects(filters);
            renderProjects(response.items);
            totalPages = response.totalPages;
            currentPage = response.page;
            renderPagination();
        } catch (err) {
            window.showNotification(err.message, 'error');
        }
    }

    function renderProjects(projects) {
        tbody.innerHTML = '';
        projects.forEach(p => {
            const row = document.createElement('tr');
            row.className = 'project-row';
            row.setAttribute('data-project-id', p.id);
            row.innerHTML = `
                <td><strong>${escapeHtml(p.name)}</strong></td>
                <td>${escapeHtml(p.category)}</td>
                <td>${escapeHtml(p.direction)}</td>
                <td>${escapeHtml(p.department)}</td>
                <td>${window.formatCurrency(p.budget)}</td>
                <td>${window.formatCurrency(p.invested)}</td>
                <td>${p.progress}%</td>
                <td>${window.formatDate(p.startDate)}</td>
            `;
            row.addEventListener('click', () => window.location.href = `project-control-detail.html?id=${p.id}`);
            tbody.appendChild(row);
        });
    }

    function renderPagination() {
        if (!paginationDiv) return;
        paginationDiv.innerHTML = `
            <div style="display: flex; justify-content: center; gap: 10px; margin-top: 1rem;">
                <button class="btn-secondary" id="prevPageBtn" ${currentPage <= 1 ? 'disabled' : ''}>← Назад</button>
                <span>Страница ${currentPage} из ${totalPages}</span>
                <button class="btn-secondary" id="nextPageBtn" ${currentPage >= totalPages ? 'disabled' : ''}>Вперёд →</button>
            </div>
        `;
        const prevBtn = document.getElementById('prevPageBtn');
        const nextBtn = document.getElementById('nextPageBtn');
        if (prevBtn) prevBtn.addEventListener('click', () => currentPage > 1 && loadProjects(currentPage - 1));
        if (nextBtn) nextBtn.addEventListener('click', () => currentPage < totalPages && loadProjects(currentPage + 1));
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

    await loadFilters();
    initDropdowns();
    await loadProjects();

    searchInput.addEventListener('input', () => loadProjects(1));
    sortSelect.addEventListener('change', () => loadProjects(1));
    dateFrom?.addEventListener('change', () => loadProjects(1));
    dateTo?.addEventListener('change', () => loadProjects(1));
});

function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/[&<>]/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;' }[m]));
}