;(function($, window) {
    $.fn.extend({
        "myChart": function (labels, datas) {
            this.each(function() {
                var dataj = {
                    labels: labels,
                    datasets: [
                        {
                            label: "价格",
                            fill: false,
                            data: datas,
                            pointRadius: 1,
                            borderWidth: 2,
                            borderColor: "rgba(226,137,100,1)"
                        }

                    ]
                };
                var options = {
                    legend: {
                        display: false,
                        labels: {

                        }
                    },
                    hover: {
                        mode: 'label',
                        intersect: false
                    },
                    tooltips: {
                        intersect: false
                    },
                    scales: {

                        xAxes: [{
                            type: 'time',
                            time: {
                                unit: 'week',
                                month: '',
                                displayFormats: {
                                    month: 'YYYY-MM',
                                    week: 'MM-DD',
                                    day: 'MM-DD'
                                }
                            }
                        }]
                    }
                };
                new Chart(this, { type: 'line', data: dataj, options: options });
            });
            //var ctx = document.getElementById(id).getContext("2d");
            
        }
    });
})(jQuery,window)