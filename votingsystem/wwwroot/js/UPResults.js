document.addEventListener("DOMContentLoaded", function () {
    const electionSelect = document.getElementById("election-select");

    function getRandomColor() {
        const colors = [
            'rgba(255, 99, 132, 0.6)', 'rgba(54, 162, 235, 0.6)',
            'rgba(255, 206, 86, 0.6)', 'rgba(75, 192, 192, 0.6)',
            'rgba(153, 102, 255, 0.6)', 'rgba(255, 159, 64, 0.6)'
        ];
        return colors[Math.floor(Math.random() * colors.length)];
    }

    function updateChart(data) {
        const container = document.getElementById("chart-container");
        container.innerHTML = `<canvas id="myChart" width="800" height="400"></canvas>`;
        const ctx = document.getElementById('myChart').getContext("2d");

        // enforce the specific order of positions
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

    function fetchLiveResults(electionId) {
        $.ajax({
            url: `/Shared/UPResults?handler=LiveResults&electionId=${electionId}`,
            type: "GET",
            success: function (data) {
                console.log("Fetched data:", data);
                updateChart(data);
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
                alert("Failed to load election results.");
            }
        });
    }

    if (electionSelect.value) {
        fetchLiveResults(electionSelect.value);
    }

    electionSelect.addEventListener("change", function () {
        fetchLiveResults(this.value);
    });
});

