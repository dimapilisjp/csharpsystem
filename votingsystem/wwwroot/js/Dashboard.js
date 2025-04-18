function updateDashboardStatistics() {
    fetch('/Dashboard/UpdateStatistics')
        .then(response => response.json())
        .then(data => {
            document.querySelector('.stat-total-elections p').innerText = data.totalElections;
            document.querySelector('.stat-total-voters p').innerText = data.totalVoters;
            document.querySelector('.stat-total-votes p').innerText = data.totalVotes;
            document.querySelector('.stat-pending-registrations p').innerText = data.pendingRegistrations;
        })
        .catch(error => console.error('Error updating dashboard statistics:', error));
}


setInterval(updateDashboardStatistics, 10000);