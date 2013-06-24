/*Global variables and functions*/

var FQTD = (function() {
    var myplace, directionsDisplay;    
    var locations = [];
    
    function isEmpty(str) {
        if ((!str || 0 === str.length) == true) {
            return '';
        }
        else {
            return str;
        }
    }

    function checkImage(str) {
        if ((!str || 0 === str.length) == true) {
            return '../images/no-image.jpg';
        }
        else {
            return str;
        }
    }
    
    function rad(x) { return x * Math.PI / 180; }

    function return_Distance(latLng1, latLng2) {
        var R = 6371; // km
        var dLat = rad(latLng2.lat() - latLng1.lat());
        var dLon = rad(latLng2.lng() - latLng1.lng());
        var lat1 = rad(latLng1.lat());
        var lat2 = rad(latLng2.lat());

        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.sin(dLon / 2) * Math.sin(dLon / 2) * Math.cos(lat1) * Math.cos(lat2);
        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        var d = R * c;
        return d * 1000;
    }
   
    return {       
        BindPropertyData: function () {
            //Bind data to checkbox
            var urlProperty = "/result/PropertyByCategoryID";
            $.getJSON(urlProperty + "?id=-1", null, function (properties) {
                for (i in properties) {
                    property = properties[i];
                    $("#property").append('<input tabindex="' + i + '" type="checkbox" id="' + property.PropertyID + '"><label for="' + property.PropertyID + '">' + property.PropertyName + '</label><br />');
                    $('#' + property.PropertyID).iCheck({
                        checkboxClass: 'icheckbox_square',
                        increaseArea: '20%' // optional
                    });
                }
            });
        },
        markOutLocation: function (lat, long, map, contentPopup) {
            var place = new google.maps.LatLng(lat, long);
            var marker = new google.maps.Marker({
                position: place,
                title: 'Click to zoom'
            });
            var infobox = new InfoBox({
                content: contentPopup,
                disableAutoPan: true,
                maxWidth: 0,
                pixelOffset: new google.maps.Size(-140, 0),
                zIndex: null,
                closeBoxMargin: "12px 4px 2px 2px",
                closeBoxURL: "/images/close.gif",
                infoBoxClearance: new google.maps.Size(1, 1)
            });
            google.maps.event.addListener(marker, 'click', function () {
                infobox.open(map, marker);
            });
            marker.setMap(map);
        },
        calcRoute: function (latitude, longitude, type) {
            var directionsService = new google.maps.DirectionsService();

            var start = myplace;
            var end = new google.maps.LatLng(latitude, longitude);
            var travelMode;

            switch (type) {
                case 'car':
                    travelMode = google.maps.DirectionsTravelMode.DRIVING;
                    break;
                case 'bike':
                    travelMode = google.maps.DirectionsTravelMode.BICYCLING;
                    break;
                case 'walk':
                    travelMode = google.maps.DirectionsTravelMode.WALKING;
                    break;
                case 'bus':
                    travelMode = google.maps.DirectionsTravelMode.TRANSIT;
                    break;
            };

            var request = {
                origin: start,
                destination: end,
                travelMode: travelMode
            };
            directionsService.route(request, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(response);
                }
            });
        },
        hidePanel: function () {
            $("#hrfClose").bind('click', function(){
                var options = {};
                $("#hrfClose").hide("fade", options, 500);
                $("#tbtable").toggle("slide", options, 500, function () {
                    var options = {};
                    $("#hrfOpen").removeClass("hidden");
                    $("#hrfOpen").show("fade", options, 500);
                });
            })
        },        
        showPanel: function () {
            $("#hrfOpen").bind('click', function(){
                var options = {};
                $("#hrfOpen").addClass("hidden");
                $("#tbtable").toggle("slide", options, 500, function () {
                    var options = {};
                    $("#hrfClose").show("fade", options, 500);
                });
            })
        },        
        displayList: function () {
            $("#tabList").bind('click', (function () {
                $("#list").removeClass("hidden");
                $("#map").addClass("hidden");

                $("#tabList").removeClass("inactive").addClass("active");
                $("#tabMap").removeClass("active").addClass("inactive");
            }));
        },
        displayMap: function () {
            $("#tabMap").bind('click', function(){
                $("#map").removeClass("hidden");
                $("#list").addClass("hidden");

                $("#tabMap").removeClass("inactive").addClass("active");
                $("#tabList").removeClass("active").addClass("inactive");
            })
        },
        pageselectCallback: function (page_index, jq) {
            // Get number of elements per pagionation page from form
            var items_per_page = 5
            var max_elem = Math.min((page_index + 1) * items_per_page, locations.length);
            var newcontent = '';

            // Iterate through a selection of the content and build an HTML string
            for (var i = page_index * items_per_page; i < max_elem; i++) {
                newcontent += '<div id="object"><table style="width: 100%;"><tr><td valign="top" style="width:116px;"><img id="photo" src="' + isEmpty(checkImage(locations[i][6])) + '" /></td><td valign="top"><h2>' + isEmpty(locations[i][3]) + '</h2>'
                    + '<p>' + isEmpty(locations[i][4]) + '<br/>' + isEmpty(locations[i][5]) + '</p><p><a href="#" class="lienket"><strong>Xem chi tiết</strong></a>'
                    + '| <a href="#" class="lienket"><strong>Đường đi</strong></a></p></td></tr></table></div>';
            }

            // Replace old content with new content
            $('#subList').html(newcontent);

            // Prevent click eventpropagation
            return false;
        },
        Pagination: function () {
            var opt = {
                callback: FQTD.pageselectCallback,
                items_per_page: 5,
                num_display_entries: 5,
                num_edge_entries: 2,
                prev_text: "Trước",
                next_text: "Sau"
            };
            console.log(FQTD.aaa);
            $("#pagination").pagination(locations.length, opt);
        },
        GetJSON: function () { //Get data result
            var urlResult = "/Result/search?";
            urlResult += "mode=" + $("#form").val();
            urlResult += "&keyword=" + $("#search").val();
            urlResult += "&currentLocation=" + $("#address").val();
            urlResult += "&categoryid=" + $("#category").val();
            urlResult += "&brandid=" + $("#brand").val();
            urlResult += "&radious=" + $("#range").val();
            urlResult += "&vn0_en1=0";

            $.getJSON(urlResult, null, function (items) {
                for (i in items) {
                    item = items[i];
                    if (item.Latitude != null && item.Longitude != null) {
                        var contentmarker = '<div class="marker"><h2>' + isEmpty(item.ItemName) + '</h2><p>' + isEmpty(item.FullAddress) + '<br/>' + isEmpty(item.Phone) + '</p></div>'
                                     + '<ul id="directionIcon">'
                                     + '<li id="car" onclick=\"calcRoute(' + item.Latitude + ',' + item.Longitude + ',\'car\')\"></li>'
                                     + '<li id="bus" onclick=\"calcRoute(' + item.Latitude + ',' + item.Longitude + ',\'bus\')\"></li>'
                                     + '<li id="walk" onclick=\"calcRoute(' + item.Latitude + ',' + item.Longitude + ',\'walk\')\"></li></ul><div id="space"></div>';
                        locations.push([item.Latitude, item.Longitude, contentmarker, isEmpty(item.ItemName), isEmpty(item.FullAddress), isEmpty(item.Phone), isEmpty(item.Logo), isEmpty(item.ItemID)]);
                    }
                }
            });
        },
        BindData: function () {
            var range = $("#range").val();

            if ($("#form").val() == "1") {
                var address = $("#address").val();
                var geocoder = new google.maps.Geocoder();
                if (geocoder) {
                    geocoder.geocode({ 'address': address }, function (results, status, content) {
                        if (status == google.maps.GeocoderStatus.OK) {
                            //set LatLng current place
                            myplace = results[0].geometry.location;

                            //set map
                            var mapProp = {
                                center: myplace,
                                disableDefaultUI: true,
                                mapTypeId: google.maps.MapTypeId.ROADMAP
                            };
                            var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

                            //set List
                            var contentList = "";

                            //set my city
                            var myCity = new google.maps.Circle({
                                center: myplace,
                                radius: parseInt(range),
                                strokeWeight: 0,
                                fillColor: "#0000FF",
                                fillOpacity: 0.1
                            });
                            myCity.setMap(map);

                            //set direction
                            directionsDisplay = new google.maps.DirectionsRenderer();
                            directionsDisplay.setMap(map);

                            map.fitBounds(myCity.getBounds());
                            FQTD.markOutLocation(myplace.lat(), myplace.lng(), map, "<p class='currentplace'>Bạn đang ở đây.</p>", myplace);

                            for (i = 0; i < locations.length; i++) {
                                var compareDistance = return_Distance(myplace, new google.maps.LatLng(locations[i][0], locations[i][1]));
                                if (range >= compareDistance) {
                                    FQTD.markOutLocation(locations[i][0], locations[i][1], map, locations[i][2], myplace);
                                }
                            }

                        } else {
                            alert("Geocode was not successful for the following reason: " + status);
                        }
                    });
                }
                $("#map").removeClass("hidden");
            }
            else {                
                $("#list").removeClass("hidden");
            }            
        },
        initResult: function () {            
            FQTD.displayList();
            FQTD.displayMap();
            FQTD.showPanel();
            FQTD.hidePanel();
            FQTD.BindPropertyData();
            FQTD.GetJSON();
            FQTD.Pagination();
            FQTD.BindData();            
        },        
    };
})();

