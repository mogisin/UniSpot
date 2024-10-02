import { MapManager } from './map.js';
import { WebSocketManager } from './app.js';

window.initMap = function() {
    const mapManager = new MapManager('map');
    mapManager.initMap();

    const webSocketManager = new WebSocketManager('ws://localhost:8000', mapManager);

    // 버튼 이벤트 설정
    document.getElementById('generateMonsters').onclick = () => webSocketManager.generateMonsters();
    document.getElementById('getAllMonsters').onclick = () => webSocketManager.getAllMonsters();
    document.getElementById('deleteAllMonsters').onclick = () => webSocketManager.deleteAllMonsters();
    document.getElementById('toggleMonsterMarkers').onclick = () => webSocketManager.toggleMonsterMarkers();
    document.getElementById('captureMonster').onclick = () => webSocketManager.captureAnyMonster();
};
