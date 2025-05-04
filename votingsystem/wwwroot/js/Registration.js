document.addEventListener("DOMContentLoaded", function () {
    // button hover effect
    let button = document.querySelector(".button input");
    if (button) {
        button.addEventListener("mouseover", () => {
            button.style.transform = "scale(1.05)";
        });
        button.addEventListener("mouseout", () => {
            button.style.transform = "scale(1)";
        });
    }

    // input focus and blur effect
    let inputs = document.querySelectorAll("input, select");
    inputs.forEach(input => {
        input.addEventListener("focus", () => {
            input.style.borderColor = "#16a085";
        });
        input.addEventListener("blur", () => {
            input.style.borderColor = "#ccc";
        });
    });

    // dropdown for department and program
    const departmentDropdown = document.getElementById('department');
    const programDropdown = document.getElementById('program');

    if (!departmentDropdown || !programDropdown) {
        console.error("Dropdown elements not found. Ensure your HTML IDs match.");
        return;
    }

    // map departments to their respective programs
    const departmentPrograms = {
        CCS: ['Bachelor of Science in Computer Science',
            'Bachelor of Science in Information Technology'
        ],
        CAS: [
            'Bachelor of Science in Psychology',
            'Bachelor of Arts in Psychology',
            'Bachelor of Arts in Communication'
        ],
        CoEng: ['Bachelor of Science in Mechanical Engineering'],
        CBAA: [
            'Bachelor of Science in Accountancy',
            'Bachelor of Science in Accounting and Information Systems',
            'Bachelor of Science in Entrepreneurship',
            'Bachelor of Science in Tourism Management'
        ],
        CoEd: ['Bachelor of Secondary Education', 'Bachelor of Elementary Education']
    };

    // Event listener to update programs when a department is selected
    departmentDropdown.addEventListener('change', (event) => {
        const selectedDepartment = event.target.value;

        // clear existing program options
        programDropdown.innerHTML = '<option value="" selected disabled>Select Program</option>';
        console.log(`Selected Department: ${selectedDepartment}`);

        // populate the program dropdown based on department
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
    });
});

