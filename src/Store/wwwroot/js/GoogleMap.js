var map;
var markers = [];
function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 49.8397, lng: 24.0297 },
        zoom: 12
    });
}

function setMarker(commonPrice) {
    var select = document.getElementById('EndPointCity');
    var city = select.options[select.selectedIndex].value;

    if (document.getElementById('EndPointStreet').value == "") {
        document.getElementById("confirmButton").style.display = "none";

        return;
    }

    document.getElementById("confirmButton").style.display = "block";
    var adress = city + ", " +
        document.getElementById('EndPointStreet').value;
    var resultlat = ''; var resultlng = '';
    $.ajax({
        async: false,
        dataType: "json",
        url: 'https://maps.google.com/maps/api/geocode/json?address=' + adress + '&key=AIzaSyCFlwlczCmamaKRfISTv2XvFJNttALOfnI',
        success: function (data) {
            for (var key in data.results) {
                resultlat = data.results[key].geometry.location.lat;
                resultlng = data.results[key].geometry.location.lng;
            }
        }
    });

    deleteMarkers();
    var marker = new google.maps.Marker({ position: { lat: resultlat, lng: resultlng }, map: map });
    markers.push(marker);
    map.setZoom(17);
    map.panTo(new google.maps.LatLng(resultlat, resultlng));
    setDeliveryPrice(commonPrice);
}

function setDeliveryPrice(commonPrice) {
    var storages = document.getElementById('avaliableStorages');
    var additonalPrice = 120;

    if (storages.length == 0) {
        document.getElementById('deliveryPrice').textContent = additonalPrice;
        document.getElementById('commonPrice').textContent = commonPrice;
        var price = Number(document.getElementById('goodPrice').textContent);
        document.getElementById('commonPrice').value = (price + additonalPrice).toString();

        return;
    }

    var select = document.getElementById('EndPointCity');
    var city = select.options[select.selectedIndex].value;
    var isTheSamePlace = false;

    for (var i = 0; i < storages.length; i++) {
        if (storages[i].value == city) {
            document.getElementById('deliveryPrice').textContent = 15;
            document.getElementById('commonPrice').textContent = commonPrice;
            var price = Number(document.getElementById('goodPrice').textContent);
            document.getElementById('commonPrice').value = (price + 15).toString();
            isTheSamePlace = true;
        }
    }

    if (!isTheSamePlace) {
        document.getElementById('deliveryPrice').textContent = additonalPrice;
        document.getElementById('commonPrice').value = commonPrice;
        var price = Number(document.getElementById('goodPrice').textContent);
        document.getElementById('commonPrice').value = (price + additonalPrice).toString();
    }
}

function deleteMarkers() {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }
    markers = [];
};