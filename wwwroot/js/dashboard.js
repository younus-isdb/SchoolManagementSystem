let attendanceChart = new Chart(document.getElementById('attendanceChart'), {
    type: 'line',
    data: { labels: [], datasets: [{ label: 'Attendance', data: [], borderColor: 'green', backgroundColor: 'rgba(0,128,0,0.1)', fill: true }] },
    options: { responsive: true, animation: { duration: 500 } }
});

let feeChart = new Chart(document.getElementById('feeChart'), {
    type: 'bar',
    data: { labels: [], datasets: [{ label: 'Fees Collected', data: [], backgroundColor: 'gold' }] },
    options: { responsive: true, animation: { duration: 500 } }
});

async function loadDashboardData() {
    const res = await fetch('/api/DashboardApi/GetDashboardData');
    const data = await res.json();

    // Update Summary Cards
    document.getElementById('totalStudents').innerText = data.TotalStudents;
    document.getElementById('totalTeachers').innerText = data.TotalTeachers;
    document.getElementById('totalAttendance').innerText = data.TotalAttendanceToday;
    document.getElementById('totalFee').innerText = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(data.TotalFeeCollectionToday);
}

// Initial load
loadDashboardData();

// SignalR Live Updates
const connection = new signalR.HubConnectionBuilder().withUrl("/dashboardHub").build();
connection.on("ReceiveDashboardUpdate", function () { loadDashboardData(); });
connection.start();


    :root {
    --primary - green: #1e5128;
    --secondary - gold: #FFD700;
}

.card { border - radius: 1rem; }
.card h5 { color: var(--primary - green); font - weight: 600; }
.card h2 { color: var(--secondary - gold); }
.btn - success { background - color: var(--primary - green); border - color: var(--primary - green); }
.btn - success:hover { background - color: darkgreen; }
