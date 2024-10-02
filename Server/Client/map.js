let map;  // 전역 변수로 선언
let monsterMarkers = {};  // 몬스터 마커들을 저장할 객체

window.initMap = function() {
    // 지도를 초기화
    map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: 37.243056, lng: 127.080063 },
        zoom: 16
    });

    const departmentCoordinates = {
        '소프트웨어융합대학': { latitude: 37.239603, longitude: 127.083157 },
        '전자정보대학': { latitude: 37.239782, longitude: 127.083313 },
        '응용과학대학': { latitude: 37.239811, longitude: 127.083476 },
        '체육대학': { latitude: 37.244493, longitude: 127.080436 },
        '공과대학': { latitude: 37.246468, longitude: 127.080844 },
        '예술디자인대학': { latitude: 37.241709, longitude: 127.084441 },
        '외국어대학': { latitude: 37.245391, longitude: 127.077649 },
        '생명과학대학': { latitude: 37.242962, longitude: 127.080932 }
    };

    const bounds = new google.maps.LatLngBounds();

    // 학과 위치에 마커 추가
    Object.keys(departmentCoordinates).forEach(department => {
        const coordinates = departmentCoordinates[department];
        
        const marker = new google.maps.Marker({
            position: { lat: coordinates.latitude, lng: coordinates.longitude },
            label: department,
            map: map
        });
        bounds.extend(marker.position);
    });

    map.fitBounds(bounds);

    // 몬스터 마커를 추가하는 함수
    window.addMonsterMarker = function(monsterId,monsterName, latitude, longitude) {
        // 이미 해당 몬스터 마커가 있는지 확인
        if (!monsterMarkers[monsterId]) {
            const marker = new google.maps.Marker({
                position: { lat: latitude, lng: longitude },
                label: monsterName,
                map: map
            });
            monsterMarkers[monsterId] = marker;  // 마커 저장
        }
    };

    // 몬스터 마커를 제거하는 함수
    window.removeMonsterMarker = function(monsterId) {
        if (monsterMarkers[monsterId]) {
            monsterMarkers[monsterId].setMap(null);  // 마커를 지도에서 제거
            delete monsterMarkers[monsterId];  // 저장된 마커 정보 삭제
        }
    };

    // 모든 몬스터 마커를 제거하는 함수
    window.removeAllMonsterMarkers = function() {
        Object.keys(monsterMarkers).forEach(monsterId => {
            monsterMarkers[monsterId].setMap(null);
            delete monsterMarkers[monsterId];
        });
    };

    // 맵 초기화 이후 WebSocket 시작
    initWebSocket();
};