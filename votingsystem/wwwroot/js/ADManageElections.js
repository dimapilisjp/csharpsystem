document.addEventListener("DOMContentLoaded", function () {
    console.log("Initializing Manage Elections script...");

    // delete button confirmation 
    document.querySelectorAll(".delete-button").forEach(button => {
        button.addEventListener("click", function (event) {
            const confirmDelete = confirm("Are you sure you want to delete this candidate?");
            if (!confirmDelete) {
                event.preventDefault();
            }
        });
    });

    // search functionality for candidates table
    const searchInput = document.getElementById("search-candidates");
    const tableRows = document.querySelectorAll(".candidate-table tbody tr");

    if (searchInput) {
        searchInput.addEventListener("input", function () {
            const searchText = this.value.toLowerCase();

            tableRows.forEach(row => {
                const rowText = row.textContent.toLowerCase();
                row.style.display = rowText.includes(searchText) ? "" : "none";
            });
        });
        console.log("Search functionality initialized.");
    } else {
        console.warn("Search input element not found.");
    }

    // dropdown for department and program
    const departmentDropdown = document.getElementById('department');
    const programDropdown = document.getElementById('program');

    if (!departmentDropdown || !programDropdown) {
        console.error("Dropdown elements not found. Ensure your HTML IDs match.");
        return;
    }

    const departmentPrograms = {
        CCS: ['Bachelor of Science in Computer Science',
            'Bachelor of Science in Information Technology'
        ],
        CAS: ['Bachelor of Science in Psychology',
            'Bachelor of Arts in Psychology',
            'Bachelor of Arts in Communication'
        ],
        CoEng: ['Bachelor of Science in Mechanical Engineering'
        ],
        CBAA: ['Bachelor of Science in Accountancy',
            'Bachelor of Science in Accounting and Information Systems',
            'Bachelor of Science in Entrepreneurship',
            'Bachelor of Science in Tourism Management'
        ],
        CoEd: ['Bachelor of Secondary Education',
            'Bachelor of Elementary Education'

        ]
    };

    // update the programs dropdown based on department
    departmentDropdown.addEventListener('change', (event) => {
        const selectedDepartment = event.target.value;
        console.log(`Selected Department: ${selectedDepartment}`);

        // clear existing program options
        programDropdown.innerHTML = '<option value="" selected disabled>Select Program</option>';

        // populate new options
        if (departmentPrograms[selectedDepartment]) {
            departmentPrograms[selectedDepartment].forEach(program => {
                const option = document.createElement('option');
                option.value = program;
                option.textContent = program;
                programDropdown.appendChild(option);
                console.log(`Added Program: ${program}`);
            });
        } else {
            console.warn(`No programs found for department: ${selectedDepartment}`);
        }

        console.log("Program dropdown updated:", programDropdown.innerHTML);
    });

    console.log("All functionality initialized successfully.");
});
