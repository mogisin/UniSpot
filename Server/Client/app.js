import { MapManager } from './map.js';

export class WebSocketManager {
    constructor(url, mapManager) {
        this.socket = new WebSocket(url);
        this.mapManager = mapManager;
        this.isMarkersVisible = false;
        this.isCapturePending = false;
        this.initWebSocket();
    }

    logMessage(message) {
        const logDiv = document.getElementById('log');
        logDiv.innerHTML += `<p>${message}</p>`;
        logDiv.scrollTop = logDiv.scrollHeight;
    }

    initWebSocket() {
        this.socket.onopen = () => {
            this.logMessage('WebSocket connection opened.');
        };

        this.socket.onmessage = (event) => {
            const data = JSON.parse(event.data);
            this.handleMessage(data);
        };

        this.socket.onclose = () => {
            this.logMessage('WebSocket connection closed.');
        };

        this.socket.onerror = (error) => {
            this.logMessage('WebSocket error: ' + error.message);
        };
    }

    handleMessage(data) {
        if (data.type === 'get_all_monsters') {
            if (this.isMarkersVisible) {
                this.showAllMonsterMarkers(data.monsters);
            }

            if (this.isCapturePending) {
                this.captureRandomMonster(data.monsters);
            }
        } else if (data.type === 'delete_all_success') {
            this.logMessage('All monsters have been deleted from the database.');
            this.mapManager.removeAllMonsterMarkers();
        }

        this.logMessage('Received from server: ' + JSON.stringify(data));
    }

    sendLocation(department) {
        const location = this.mapManager.departmentCoordinates[department];
        if (location) {
            const message = {
                type: 'location',
                username: 'testUser',
                latitude: location.latitude,
                longitude: location.longitude
            };
            this.socket.send(JSON.stringify(message));
            this.logMessage('Sent location for ' + department + ': ' + JSON.stringify(message));
        }
    }

    generateMonsters() {
        const message = {
            type: 'generate_monsters',
            count: 1
        };
        this.socket.send(JSON.stringify(message));
        this.logMessage('Requested to generate random monsters.');
    }

    getAllMonsters() {
        const message = {
            type: 'get_all_monsters'
        };
        this.socket.send(JSON.stringify(message));
        this.logMessage('Requested to get all monsters.');
    }

    deleteAllMonsters() {
        const message = {
            type: 'delete_all'
        };
        this.socket.send(JSON.stringify(message));
        this.logMessage('Requested to delete all monsters.');
    }

    captureAnyMonster() {
        this.logMessage('captureAnyMonster button clicked');
        this.isCapturePending = true;
        this.getAllMonsters();
    }

    showAllMonsterMarkers(monsters) {
        monsters.forEach(monster => {
            this.mapManager.addMonsterMarker(monster._id, monster.name, monster.location.latitude, monster.location.longitude);
        });
        this.logMessage('All monster markers added.');
    }

    toggleMonsterMarkers() {
        if (this.isMarkersVisible) {
            this.mapManager.removeAllMonsterMarkers();
        } else {
            this.getAllMonsters();
        }
        this.isMarkersVisible = !this.isMarkersVisible;
    }

    captureRandomMonster(monsters) {
        if (monsters.length > 0) {
            const randomMonster = monsters[Math.floor(Math.random() * monsters.length)];
            const captureMessage = {
                type: 'capture',
                username: 'testUser',
                monsterId: randomMonster._id
            };
            this.socket.send(JSON.stringify(captureMessage));
            this.logMessage(`Requested to capture monster with ID: ${randomMonster._id}`);
        } else {
            this.logMessage('No monsters available to capture.');
        }
        this.isCapturePending = false;
    }
}
