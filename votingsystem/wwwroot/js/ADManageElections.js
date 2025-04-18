document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".delete-button").forEach(button => {
        button.addEventListener("click", function (event) {
            const confirmDelete = confirm("Are you sure you want to delete this candidate?");
            if (!confirmDelete) {
                event.preventDefault();
            }
        });
    });

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
    }
});
