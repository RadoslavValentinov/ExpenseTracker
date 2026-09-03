let expensesChart = null;
let categoryChart = null;


window.renderExpensesChart = function (labels, values) {

    const canvas = document.getElementById("expensesChart");

    if (!canvas) {
        console.warn("expensesChart canvas not found.");
        return;
    }

    if (typeof Chart === "undefined") {
        console.error("Chart.js is not loaded.");
        return;
    }

    if (expensesChart !== null) {
        expensesChart.destroy();
    }


    expensesChart = new Chart(canvas, {

        type: "bar",

        data: {

            labels: labels,

            datasets: [{
                label: "Разходи",
                data: values,
                borderWidth: 0,
                borderRadius: 8,
                maxBarThickness: 55
            }]
        },

        options: {

            responsive: true,

            maintainAspectRatio: false,

            plugins: {

                legend: {
                    display: false
                },

                tooltip: {

                    callbacks: {

                        label: function (context) {

                            const value =
                                Number(context.raw || 0);

                            return " " +
                                value.toLocaleString(
                                    "bg-BG",
                                    {
                                        minimumFractionDigits: 2,
                                        maximumFractionDigits: 2
                                    }
                                ) +
                                " лв.";
                        }
                    }
                }
            },

            scales: {

                y: {

                    beginAtZero: true,

                    ticks: {

                        callback: function (value) {

                            return Number(value).toLocaleString(
                                "bg-BG"
                            ) + " лв.";
                        }
                    }
                },

                x: {

                    grid: {
                        display: false
                    }
                }
            }
        }
    });
};



window.renderCategoryChart = function (labels, values) {

    const canvas =
        document.getElementById("categoryChart");

    if (!canvas) {
        console.warn("categoryChart canvas not found.");
        return;
    }

    if (typeof Chart === "undefined") {
        console.error("Chart.js is not loaded.");
        return;
    }

    if (categoryChart !== null) {
        categoryChart.destroy();
    }


    categoryChart = new Chart(canvas, {

        type: "doughnut",

        data: {

            labels: labels,

            datasets: [{

                data: values,

                borderWidth: 2,

                hoverOffset: 8
            }]
        },

        options: {

            responsive: true,

            maintainAspectRatio: false,

            cutout: "62%",

            plugins: {

                legend: {

                    position: "bottom",

                    labels: {

                        padding: 18,

                        usePointStyle: true,

                        pointStyle: "circle"
                    }
                },

                tooltip: {

                    callbacks: {

                        label: function (context) {

                            const value =
                                Number(context.raw || 0);

                            return " " +
                                value.toLocaleString(
                                    "bg-BG",
                                    {
                                        minimumFractionDigits: 2,
                                        maximumFractionDigits: 2
                                    }
                                ) +
                                " лв.";
                        }
                    }
                }
            }
        }
    });
};