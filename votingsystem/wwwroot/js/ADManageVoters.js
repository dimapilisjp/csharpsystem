document.querySelectorAll(".delete-button").forEach(button => {
    button.addEventListener("click", function (event) {
        const confirmDelete = confirm("Are you sure you want to delete this voter?");
        if (!confirmDelete) {
            event.preventDefault(); 
        }
    });
});


document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("search-voters"); 
    const tableRows = document.querySelectorAll(".voter-table tbody tr");

    if (searchInput) {
        searchInput.addEventListener("input", function () {
            const searchText = this.value.toLowerCase();

            tableRows.forEach(row => {
                const rowText = row.textContent.toLowerCase();
                row.style.display = rowText.includes(searchText) ? "" : "none"; 
        });
    }
});
