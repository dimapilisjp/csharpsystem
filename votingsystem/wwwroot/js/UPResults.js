document.addEventListener('DOMContentLoaded', () => {
    console.log("JavaScript is loaded!"); // Debugging

    // Ensure electionData is passed from Razor
    const electionData = [
        { ElectionId: 1, CandidateName: "Superman", VoteCount: 2 },
        { ElectionId: 1, CandidateName: "Spiderman1", VoteCount: 1 },
        { ElectionId: 1, CandidateName: "Robin", VoteCount: 1 },
        { ElectionId: 1, CandidateName: "Ewan", VoteCount: 1 }
    ]; // Replace this with Razor-generated data for now

    if (!electionData || electionData.length === 0) {
        console.error("ElectionData is empty or undefined!");
        return;
    }
    console.log("ElectionData:", electionData);

    // group data by their ElectionId
    const groupedData = electionData.reduce((acc, item) => {
        acc[item.ElectionId] = acc[item.ElectionId] || [];
        acc[item.ElectionId].push(item);
        return acc;
    }, {});

    console.log("Grouped Data:", groupedData); // debug grouped data

    // make charts for each election
    Object.keys(groupedData).forEach(electionId => {
        const ctx = document.getElementById(`chart-${electionId}`);
        if (!ctx) {
            console.error(`Canvas element for ElectionId ${electionId} not found!`);
            return;
        }

        const data = groupedData[electionId];
        const chartData = {
            labels: data.map(item => item.CandidateName),
            datasets: [{
                label: "Vote Count",
                data: data.map(item => item.VoteCount),
                backgroundColor: "rgba(75, 192, 192, 0.2)",
                borderColor: "rgba(75, 192, 192, 1)",
                borderWidth: 1
            }]
        };

        // create bar chart
        new Chart(ctx.getContext("2d"), {
            type: "bar",
            data: chartData,
            options: {
                responsive: true,
                maintainAspectRatio: true, // Ensures proper scaling
                aspectRatio: 2,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    });
});
