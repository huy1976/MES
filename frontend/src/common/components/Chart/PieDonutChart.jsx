import ReactApexChart from "react-apexcharts"
import React from "react"

function PieDonutChart({ height, width, name, label, unit, fontSize, dataChartLabels, dataChartValue, arrayColors }) {
    const setUpChart = {
        options: {
            chart: {
                type: "donut",
            },
            labels: dataChartLabels,
            colors: arrayColors,
            legend: {
                show: false,
                position: "right", // Vị trí của chú thích
                formatter: function (label, series) {
                    return `${label}: ${setUpChart.series[series.seriesIndex]}` // Hiển thị nhãn + số lượng
                },
            },
            plotOptions: {
                pie: {
                    expandOnClick: true,
                    donut: {
                        size: "60%",
                        labels: {
                            show: true,
                            total: {
                                show: true,
                                showAlways: true,
                                fontSize: fontSize * 0.9,
                                color: "black",
                                label: label,
                            },
                        },
                    },
                },
            },
            dataLabels: {
                enabled: true,
                style: {
                    fontSize: fontSize * 0.6,
                },
                formatter: function (val) {
                    return val.toFixed(0) + "%"
                },
            },
            tooltip: {
                y: {
                    formatter: function (val) {
                        return `${val} ${unit}` // Thêm đơn vị phía sau số
                    },
                },
                style: {
                    fontSize: fontSize, // Chỉnh kích thước font chữ
                    fontFamily: "Arial, sans-serif", // Tuỳ chỉnh font chữ nếu cần
                },
            },
            title: {
                text: String(name),
                align: "center",
                fontSize: fontSize,
                fontFamily: "Arial, sans-serif",
            },
        },
        series: dataChartValue,
    }
    return (
        <>
            <ReactApexChart
                options={setUpChart.options}
                series={setUpChart.series}
                type="donut"
                width={width}
                height={height}
            />
        </>
    )
}
export default PieDonutChart
