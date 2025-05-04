document.addEventListener("DOMContentLoaded", function () {
    document.querySelector(".votes-table").addEventListener("click", function (event) {
        if (event.target.classList.contains("view-ballot-button")) {
            const userId = event.target.getAttribute("data-userid");
            const electionId = event.target.getAttribute("data-electionid");

            fetch(`/Shared/ADVotesRecord?handler=VoteDetails&userId=${userId}&electionId=${electionId}`)
                .then(response => response.json())
                .then(data => {
                    displayBallot(data, electionId);
                })
                .catch(error => {
                    console.error("Error fetching ballot details:", error);
                });
        }
    });

    function displayBallot(data, electionId) {
        document.getElementById("receipt-election-title").textContent = `Election ${electionId}`;
        const votedContainer = document.getElementById("voted-container");
        votedContainer.innerHTML = "";

        if (!data || data.Selections.length === 0) {
            votedContainer.innerHTML = "<p>No ballot found for this election.</p>";
            return;
        }

        const groupedCandidates = {};
        data.Selections.forEach(item => {
            if (!groupedCandidates[item.Position]) {
                groupedCandidates[item.Position] = [];
            }
            groupedCandidates[item.Position].push(item.CandidateName);
        });

        for (const [position, candidates] of Object.entries(groupedCandidates)) {
            const positionGroup = document.createElement("div");
            positionGroup.classList.add("position-group");
            positionGroup.innerHTML = `<h3>Position: ${position}</h3>`;

            const candidateList = document.createElement("ul");
            candidates.forEach(name => {
                const listItem = document.createElement("li");
                listItem.textContent = name;
                candidateList.appendChild(listItem);
            });

            positionGroup.appendChild(candidateList);
            votedContainer.appendChild(positionGroup);
        }

        document.getElementById("ballot-details").classList.remove("hidden");
    }
});
