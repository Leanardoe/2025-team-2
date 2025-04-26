document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchInput');
    const dropdownMenu = document.querySelector('.dropdown-menu'); // <ul class="dropdown-menu">
    const selectedSkillsList = document.getElementById('selectedSkills');
    const skillIdsInput = document.getElementById('skillIdsInput');

    //HEADER
    const table = document.querySelector('table');
    const headers = table.querySelectorAll('th[data-column]');
    const tbody = table.querySelector('tbody');


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

        if (filter.length === 0 || (filter.length > 0 && hasMatches)) {
            dropdownMenu.classList.add('show');
        } else {
            dropdownMenu.classList.remove('show');
        }
    });

    function updateSkillIds() {
        // Join the selected skill IDs array into a comma-separated string
        skillIdsInput.value = Array.from(selectedSkillsList.querySelectorAll('button')).map(tag => tag.getAttribute('data-id')).join(',');
    }

    function updatePersistantSkills() {
        const skillIdsInput = document.getElementById("skillIdsInput");
        const selectedSkills = document.querySelectorAll(".persistent-tag");

        // Ensure that only valid tags with data-value are considered
        const skillIds = Array.from(selectedSkills)
            .map(tag => tag.getAttribute("data-value"))
            .filter(value => value); // Filter out any empty values

        // Update the hidden input with the comma-separated skill IDs
        skillIdsInput.value = skillIds.join(",");
    }

    // Add click event for each dropdown item
    dropdownItems.forEach(item => {
        item.addEventListener('click', function (event) {
            event.preventDefault();

            const value = item.getAttribute('data-value');
            const text = item.textContent;

            // Check if the skill is already selected by checking if it exists in the list of tags
            if (!Array.from(selectedSkillsList.children).some(tag => tag.getAttribute('data-id') === value)) {
                // Create a new tag for the selected skill
                const skillTag = document.createElement('button');
                skillTag.className = 'btn btn-outline-danger btn-sm-tag';
                skillTag.textContent = text; // Display the text
                skillTag.setAttribute('data-id', value); // Store the value in the tag's data-id

                // Add click event to remove the skill from the list
                skillTag.addEventListener('click', function () {
                    // Remove the tag from the UI
                    selectedSkillsList.removeChild(skillTag);
                    updateSkillIds(); // Update the hidden input after removal
                });

                // Append the skill tag to the selected skills list
                selectedSkillsList.appendChild(skillTag);
                updateSkillIds(); // Update the hidden input with the new selected skill ID
            }
        });
    });

    // Hide dropdown when clicking outside of the input or dropdown
    document.addEventListener('click', function (event) {
        const isClickInside = searchInput.contains(event.target) || dropdownMenu.contains(event.target);
        if (!isClickInside) {
            dropdownMenu.classList.remove('show');
            searchInput.blur();
        }
    });

    searchInput.addEventListener('focus', function () {
        const filter = searchInput.value.toLowerCase();
        let hasMatches = false;

        dropdownItems.forEach(item => {
            const text = item.textContent.toLowerCase();
            if (text.includes(filter)) {
                item.style.display = '';
                hasMatches = true;
            } else {
                item.style.display = 'none';
            }
        });

        // If nothing is typed, show the dropdown with all items, else show the filtered results
        if (filter.length === 0 || (filter.length > 0 && hasMatches)) {
            dropdownMenu.classList.add('show');
        }
    });

    headers.forEach(header => {
        const arrowSpan = header.querySelector('.sort-arrow');
        const columnIndex = parseInt(header.getAttribute('data-column'));

        header.addEventListener('click', () => {
            const currentOrder = header.getAttribute('data-order');
            const newOrder = currentOrder === 'asc' ? 'desc' : 'asc';
            header.setAttribute('data-order', newOrder);

            // Reset all other arrows
            headers.forEach(h => {
                const span = h.querySelector('.sort-arrow');
                if (span) span.textContent = '';
            });

            // Set new arrow
            arrowSpan.textContent = newOrder === 'asc' ? '▲' : '▼';

            // Sort rows
            const rows = Array.from(tbody.querySelectorAll('tr'));
            rows.sort((a, b) => {
                let aVal, bVal;

                if (header.textContent.includes('Match')) { // Match % column
                    aVal = parseFloat(a.querySelector('[aria-valuenow]')?.getAttribute('aria-valuenow') || '0');
                    bVal = parseFloat(b.querySelector('[aria-valuenow]')?.getAttribute('aria-valuenow') || '0');
                } else {
                    aVal = a.children[columnIndex].textContent.trim().toLowerCase();
                    bVal = b.children[columnIndex].textContent.trim().toLowerCase();
                }

                if (typeof aVal === 'number' && typeof bVal === 'number') {
                    return newOrder === 'asc' ? aVal - bVal : bVal - aVal;
                } else {
                    return newOrder === 'asc'
                        ? aVal.localeCompare(bVal)
                        : bVal.localeCompare(aVal);
                }
            });

            rows.forEach(row => tbody.appendChild(row));
        });
    });

    const skillTags = document.querySelectorAll(".persistent-tag");

    skillTags.forEach(function (skillTag) {
        skillTag.addEventListener("click", function () {
            // Remove the tag from the UI
            skillTag.parentNode.removeChild(skillTag);
            updateSkillIds(); // Update the hidden input after removal
        });
    });

    // Update hidden input when form is submitted
    document.getElementById('filterForm').addEventListener('submit', function () {
        updateSkillIds();// Ensure the selected skill IDs are in the hidden input before submitting
    });


});