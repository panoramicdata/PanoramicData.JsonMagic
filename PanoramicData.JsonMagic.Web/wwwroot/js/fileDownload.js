window.downloadFileFromStream = async (fileName, contentStreamReference) => {
	const arrayBuffer = await contentStreamReference.arrayBuffer();
	const blob = new Blob([arrayBuffer]);
	const url = URL.createObjectURL(blob);
	const anchorElement = document.createElement('a');
	anchorElement.href = url;
	anchorElement.download = fileName ?? '';
	anchorElement.click();
	anchorElement.remove();
	URL.revokeObjectURL(url);
}

window.downloadApexChart = function (chartId, fileName) {
	var chart = ApexCharts.getChartByID(chartId);
	if (chart) {
		chart.dataURI().then((uriObj) => {
			const uri = uriObj.imgURI;
			let a = document.createElement('a');
			a.href = uri;
			a.download = fileName + '.png';
			a.click();
			a.remove();
		});
	}
}