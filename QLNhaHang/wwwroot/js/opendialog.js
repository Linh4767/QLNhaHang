function initializeFilterDialog() {
    const filterItems = document.querySelectorAll('.menu-filters li');
    const openFilterDialog = document.getElementById('openFilterDialog');
    const filterDialog = document.getElementById('filterDialog');
    const modalBody = filterDialog.querySelector('.modal-body');
    const closeModal = document.getElementById('closeModalMenu');
    let selectedFilter = '*'; // Default filter

    // Lắng nghe sự kiện click trên menu filter
    filterItems.forEach(item => {
        item.addEventListener('click', function () {
            filterItems.forEach(filter => filter.classList.remove('filter-active'));
            item.classList.add('filter-active');
            selectedFilter = item.getAttribute('data-filter');
            applyFilter(selectedFilter);
        });
    });

    // Mở modal và hiển thị danh sách món ăn được lọc
    openFilterDialog.addEventListener('click', function (event) {
        event.preventDefault();

        // Đảm bảo modal-body được reset
        modalBody.innerHTML = ''; // Xóa sạch nội dung cũ trước

        // Lấy lại danh sách item theo filter hiện tại
        const menuItems = document.querySelectorAll('.menu-item');
        const filteredItems = Array.from(menuItems).filter(item =>
            selectedFilter === '*' || item.classList.contains(selectedFilter.substring(1))
        );

        // Hiển thị nội dung trong modal
        if (filteredItems.length > 0) {
            modalBody.innerHTML = filteredItems.map(item => item.outerHTML).join('');
        } else {
            modalBody.innerHTML = '<p>Không có món ăn nào thuộc danh mục này.</p>';
        }

        // Hiển thị modal
        filterDialog.style.display = 'block';
        filterDialog.classList.add('show');

        // Khóa cuộn trang nền
        document.body.classList.add('modal-open');
    });



    // Đóng modal
    closeModal.addEventListener('click', function () {
        filterDialog.classList.remove('show');
        filterDialog.style.display = 'none'; // Ẩn modal

        // Khôi phục cuộn trang
        document.body.classList.remove('modal-open');
    });
}

// Hàm áp dụng bộ lọc trực tiếp trên danh sách hiển thị
function applyFilter(filter) {
    const menuItems = document.querySelectorAll('.menu-item');
    menuItems.forEach(item => {
        if (filter === '*' || item.classList.contains(filter.substring(1))) {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });
}

// Gọi hàm khởi tạo
initializeFilterDialog();
