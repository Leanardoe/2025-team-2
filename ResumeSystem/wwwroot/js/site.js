document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchInput');
    const dropdownMenu = document.querySelector('.dropdown-menu'); // <ul class="dropdown-menu">

    // Initially, hide the dropdown menu
    dropdownMenu.classList.remove('show');

    // Get all dropdown items (these can be dynamically populated)
    const dropdownItems = document.querySelectorAll('.dropdown-item');

    // Add the search input filter functionality
    searchInput.addEventListener('input', function () {
        const filter = searchInput.value.toLowerCase();
        let hasMatches = false;

        dropdownItems.forEach(function (item) {
            const text = item.textContent.toLowerCase();
            if (text.includes(filter)) {
                item.style.display = '';
                hasMatches = true;
            } else {
                item.style.display = 'none';
            }
        });

        if (filter.length > 0 && hasMatches) {
            dropdownMenu.classList.add('show');
        } else {
            dropdownMenu.classList.remove('show');
        }
    });

    // Add event listener for each dropdown item click
    dropdownItems.forEach(item => {
        item.addEventListener('click', function (event) {
            event.preventDefault();

            // Get the value and text of the clicked item
            const value = item.getAttribute('data-value');
            const text = item.textContent;

            // Do something with the value and text
            console.log('Skill selected:', value, text);
        });
    });
});