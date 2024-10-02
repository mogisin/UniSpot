export class MapManager {
    constructor(mapElementId) {
        this.mapElementId = mapElementId;
        this.map = null;
        this.monsterMarkers = {};
        this.departmentCoordinates = {
            '소프트웨어융합대학': { latitude: 37.239603, longitude: 127.083157 },
            '전자정보대학': { latitude: 37.239782, longitude: 127.083313 },
            '응용과학대학': { latitude: 37.239811, longitude: 127.083476 },
            '체육대학': { latitude: 37.244493, longitude: 127.080436 },
            '공과대학': { latitude: 37.246468, longitude: 127.080844 },
            '예술디자인대학': { latitude: 37.241709, longitude: 127.084441 },
            '외국어대학': { latitude: 37.245391, longitude: 127.077649 },
            '생명과학대학': { latitude: 37.242962, longitude: 127.080932 }
        };
    }

    initMap() {
        this.map = new google.maps.Map(document.getElementById(this.mapElementId), {
            center: { lat: 37.243056, lng: 127.080063 },
            zoom: 16
        });

        const bounds = new google.maps.LatLngBounds();

        // 학과 위치에 마커 추가
        Object.keys(this.departmentCoordinates).forEach(department => {
            const coordinates = this.departmentCoordinates[department];

            const marker = new google.maps.Marker({
                position: { lat: coordinates.latitude, lng: coordinates.longitude },
                label: department,
                map: this.map
            });
            bounds.extend(marker.position);
        });

        this.map.fitBounds(bounds);
    }

    addMonsterMarker(monsterId, monsterName, latitude, longitude) {
        if (!this.monsterMarkers[monsterId]) {
            const marker = new google.maps.Marker({
                position: { lat: latitude, lng: longitude },
                label: monsterName,
                map: this.map
            });
            this.monsterMarkers[monsterId] = marker;
        }
    }

    removeMonsterMarker(monsterId) {
        if (this.monsterMarkers[monsterId]) {
            this.monsterMarkers[monsterId].setMap(null);
            delete this.monsterMarkers[monsterId];
        }
    }

    removeAllMonsterMarkers() {
        Object.keys(this.monsterMarkers).forEach(monsterId => {
            this.removeMonsterMarker(monsterId);
        });
    }
}
