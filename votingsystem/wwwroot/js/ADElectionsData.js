document.addEventListener("DOMContentLoaded", function () {
    const electionSelect = document.getElementById("election-dropdown");

    function getRandomColor() {
        const colors = [
            'rgba(255, 99, 132, 0.6)', 'rgba(54, 162, 235, 0.6)',
            'rgba(255, 206, 86, 0.6)', 'rgba(75, 192, 192, 0.6)',
            'rgba(153, 102, 255, 0.6)', 'rgba(255, 159, 64, 0.6)'
        ];
        return colors[Math.floor(Math.random() * colors.length)];
    }

    function updateChart(data) {
        const container = document.getElementById("vote-chart-container");
        container.innerHTML = `<canvas id="voteChart" width="800" height="400"></canvas>`;
        const ctx = document.getElementById('voteChart').getContext("2d");

        const orderedPositions = ["President", "Vice President", "Secretary", "Treasurer", "Auditor", "PRO"];
        const positions = orderedPositions.filter(pos => data.some(x => x.position === pos));
        const candidateNames = [...new Set(data.map(x => x.candidateName))];

        const datasets = candidateNames.map(name => {
            return {
                label: name,
                data: positions.map(pos => {
                    const found = data.find(d => d.position === pos && d.candidateName === name);
                    return found ? found.voteCount : 0;
                }),
                backgroundColor: getRandomColor()
            };
        });

        new Chart(ctx, {
            type: "bar",
            data: {
                labels: positions,
                datasets: datasets
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top'
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                return `${context.dataset.label}: ${context.parsed.y} votes`;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        title: {
                            display: true,
                            text: 'Position'
                        }
                    },
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Votes'
                        }
                    }
                }
            }
        });
    }

    function updateVoterTable(stats) {
        const tbody = document.getElementById("voter-stats");
        tbody.innerHTML = "";
        stats.forEach(row => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${row.department}</td>
                <td>${row.program}</td>
                <td>${row.totalVoters}</td>
                <td>${row.usersVoted}</td>
                <td>${row.votePercentage}%</td>
            `;
            tbody.appendChild(tr);
        });
    }

    function updateCandidateTable(data) {
        const tbody = document.getElementById("candidate-stats");
        tbody.innerHTML = "";
        data.forEach(row => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${row.position}</td>
                <td>${row.candidateName}</td>
                <td>${row.voteCount}</td>
                <td>${row.votePercentage}%</td>
            `;
            tbody.appendChild(tr);
        });
    }

    function fetchElectionData(electionId) {
        fetch(`/Shared/ADElectionsData?handler=ElectionData&electionId=${electionId}`)
            .then(response => response.json())
            .then(data => {
                console.log("Election data:", data);
                updateVoterTable(data.voterStatistics);
                updateCandidateTable(data.voteCompare);
                updateChart(data.voteCompare);
            })
            .catch(error => {
                console.error("Error fetching election data:", error);
                alert("Failed to load election data.");
            });
    }

    if (electionSelect && electionSelect.value) {
        fetchElectionData(electionSelect.value);
    }

    if (electionSelect) {
        electionSelect.addEventListener("change", function () {
            fetchElectionData(this.value);
        });
    }
});
