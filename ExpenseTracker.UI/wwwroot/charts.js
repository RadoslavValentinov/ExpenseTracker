let expensesChart = null;

window.renderExpensesChart = (labels, values) => {

    const ctx =
        document.getElementById('expensesChart');

    if (!ctx)
        return;

    // destroy old chart

    if (expensesChart !== null) {
        expensesChart.destroy();
    }

    expensesChart = new Chart(ctx, {

        type: 'bar',

        data: {

            labels: labels,

            datasets: [{

                label: 'Expenses',

                data: values,

                borderWidth: 1
            }]
        },

        options: {

            responsive: true,

            maintainAspectRatio: false
        }
    });
}