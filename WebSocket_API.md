# WebSocket API Documentation

## 개요 (Overview)
이 API는 WebSocket을 통해 클라이언트와 서버 간에 실시간으로 몬스터 생성, 삭제, 포획 및 위치 업데이트를 할 수 있도록 합니다. 클라이언트는 다양한 유형의 메시지를 서버로 전송하고, 서버는 요청에 따라 적절한 응답을 반환합니다.

## WebSocket 연결
- **서버 URL**: `ws://___.___.___.___:8000`
- 클라이언트는 이 URL을 사용하여 서버와 WebSocket 연결을 수립할 수 있습니다.

## 메시지 구조
모든 메시지는 JSON 형식이며, 각 메시지에는 요청의 유형을 나타내는 `type` 필드가 포함됩니다.

### 예시
```json
{
  "type": "<수신 메시지 유형>",
  "username": "<유저의 이름>",
  "additional_data": { ... }
}
```
## 용어 정의
- **몬스터(Monster)**: 게임 내에서 포획할 수 있는 객체.
- **유저(User)**: 게임을 플레이하는 사용자.
- **스팟(Spot)**: 특정 위치를 의미하며, 유저가 `userPoint`를 사용해 점령 가능
---



## 수신 메시지 유형 (Message Types)

### 1. **generate_monsters**
- **설명**: 일정 수의 랜덤 몬스터를 생성합니다.   
count는 선택 사항이며, 포함되지 않을 경우 기본값으로 1이 설정됩니다.


- **수신 메시지 형식**:
```json
{
  "type": "generate_monsters",
  "count": 5 //선택 사항, 기본값: 1
}
```
- **서버에서 수행하는 작업**: `count`에 따라 새로운 몬스터를 생성하고 데이터베이스에 저장한 후 클라이언트에 성공 메시지를 반환합니다.  


- **서버 응답 형식**:
```json
{
  "type": "generate_monsters_success",
  "message": "5 monsters created successfully.",
  "monsters": [
    {
      "_id": "monster1",
      "name": "소융대몬_612",
      "location": { "latitude": 37.5, "longitude": 127.0 }
    },
    ...
  ]
}
```

### 2. **location**
- **설명**: 유저의 위치 정보를 서버에 업데이트하고, 근처의 몬스터를 반환합니다.
- **수신 메시지 형식**:
```json
{
  "type": "location",
  "username": "testUser",
  "latitude": 37.5,
  "longitude": 127.0
}
```
- **서버 응답 형식**:
```json
{
  "type": "nearby_monsters",
  "monsters": [
    {
      "_id": "monster1",
      "name": "소융대몬612",
      "location": { "latitude": 37.5, "longitude": 127.0 }
    },
    ...
  ]
}
```

### 3. **updateLocation**
- **설명**: 유저의 위치를 업데이트합니다.
- **수신 메시지 형식**:
```json
{
  "type": "updateLocation",
  "username": "testUser",
  "latitude": 37.5,
  "longitude": 127.0
}
```

### 4. **capture**
- **설명**: 특정 몬스터를 포획합니다.
- **수신 메시지 형식**:
```json
{
  "type": "capture",
  "username": "testUser",
  "monsterId": "monster1"
}
```
- **서버 응답 형식**:
```json
{
  "type": "capture_success",
  "monster": "소융대몬_612",
  "message": "Monster captured successfully."
}
```

### 5. **get_all_monsters**
- **설명**: 모든 포획되지 않은 몬스터의 목록을 요청합니다.
- **수신 메시지 형식**:
```json

{
  "type": "get_all_monsters"
}
```
- **서버 응답 형식**:
```json
{
  "type": "get_all_monsters",
  "monsters": [
    {
      "_id": "monster1",
      "name": "소융대몬_612",
      "location": { "latitude": 37.5, "longitude": 127.0 }
    },
    ...
  ]
}
```

### 6. **delete_all**
- **설명**: 데이터베이스에 저장된 모든 몬스터와 유저의 몬스터 목록을 삭제합니다.
- **수신 메시지 형식**:
```json
{
  "type": "delete_all"
}
```
- **서버 응답 형식**:
```json
{
  "type": "delete_all_success",
  "message": "All monsters and user monster lists have been deleted."
}
```

### 7. **delete_monster**
- **설명**: 특정 몬스터를 삭제하고, 모든 유저의 몬스터 목록에서 해당 몬스터를 제거합니다.
- **수신 메시지 형식**:
```json
{
  "type": "delete_monster",
  "monsterId": "monster1"
}
```
- **서버 응답 형식**:
```json
{
  "type": "delete_monster_success",
  "monsterId": "monster1",
  "message": "Monster deleted successfully."
}
```

### 8. **capture_spot**
- **설명**: 특정 스팟을 포획합니다.
- **수신 메시지 형식**:
```json
{
  "type": "capture_spot",
  "username": "testUser",
  "spotName": "소프트웨어융합대학"
}
```


### 9. **get_nearby_monsters**
- **설명**: 유저의 현재 위치 근처의 몬스터를 요청합니다.
- **수신 메시지 형식**:
```json
{
  "type": "get_nearby_monsters",
  "username": "testUser"
}
```
- **서버 응답 형식**:
```json
{
  "type": "res_nearby_monsters",
  "monsters": [
    {
      "_id": "monster1",
      "name": "소융대몬_612",
      "location": { "latitude": 37.5, "longitude": 127.0 }
    },
    ...
  ]
}
```
### 10. **get_user_money**
- **설명**: 유저의 현재 money 상태를 요청합니다.
- **수신 메시지 형식**:
```json
{
  "type": "get_user_money",
  "username": "testUser"
}
```
- **서버 응답 형식**:
```json
{
  "type": "res_user_money",
  "username": "testUser",
  "money":100
}
```

---

<!-- ## 응답 유형 (Response Types)
각 요청에 대한 응답은 위의 응답 형식 섹션에서 자세히 설명되어 있습니다.

--- -->

## 에러 처리
- 서버는 잘못된 요청이나 처리 중 오류가 발생한 경우 에러 메시지를 반환합니다.
- **에러 응답 형식**:
```json
{
  "type": "error",
  "message": "Error description."
}
```
- **예시**:
```json
{
  "type": "error",
  "message": "Monster with ID 'monster1' not found."
}
```

---

## 클라이언트-서버 통신 흐름

### 예시 1: 근처 몬스터 찾기

1. **클라이언트 요청**:
```json
{
  "type": "get_nearby_monsters",
  "username": "testUser"
}
```
2. **서버 응답**:
```json
{
  "type": "res_nearby_monsters",
  "monsters": [
    {
      "_id": "monster1",
      "name": "Pikachu",
      "location": { "latitude": 37.5, "longitude": 127.0 }
    }
  ]
}
```

### 예시 2: 몬스터 포획

1. **클라이언트 요청**:
```json
{
  "type": "capture",
  "username": "testUser",
  "monsterId": "monster1"
}
```
- **서버에서 수행하는 작업**: 몬스터의 `captured` 값을 `true`로 변경, 유저 보유 몬스터 목록 업데이트
2. **서버 응답**:
```json
{
  "type": "capture_success",
  "monster": "소융대몬_612",
  "message": "Monster captured successfully."
}
```
### 예시 3: 스팟 점령

1. **클라이언트 요청**:
```json
{
  "type": "capture_spot",
  "spotName": "소프트웨어융합대학",
  "username": "testUser",
}
```
- **서버에서 수행하는 작업**: 유저가 현재 보유중인 몬스터 수 `userPoint`가 Spot의 `lastPoint`보다 클 경우 스팟 capture 성공
2. **서버 응답**:
```json
{
  "type": "capture_spot_success",
  "spotName": "소프트웨어융합대학",
  "username": "testUser"
}
```

---

## 제약 사항
- **몬스터 생성**: 한 번에 생성할 수 있는 몬스터의 최대 수는 10마리로 제한됩니다.
- **위치 정보 정확도**: 몬스터와 유저의 위치는 소수점 이하 6자리까지 지원됩니다.
- **포획 범위**: 몬스터를 포획하기 위해서는 몬스터와의 거리가 10미터 이내여야 합니다. (GPS 기준)

---

## 데이터 모델

### 몬스터 (Monster)
- **필드**:
  - `_id`: 몬스터의 고유 ID
  - `name`: 몬스터의 이름
  - `dept`: 몬스터의 소속 단과대
  - `generateSpot`: 소속 단과대와 별개인 생성 위치 (ex:중앙도서관)
  - `tags` : 태그 `(Array)`

  - `location`: 몬스터의 위치 `(Object)`
    - `latitude`: 위도
    - `longitude`: 경도
  - `captured`: 포획 여부 (`true` 또는 `false`)

### 유저 (User)
- **필드**:
  - `username`: 유저의 이름
  - `money`: 유저의 보유 금액
  - `location`: 유저의 위치 `(Object)`
    - `latitude`: 위도
    - `longitude`: 경도
  - `monsters`: 유저가 포획한 몬스터 목록 `(Array)`
    - `name`: 몬스터 이름
    - `capturedAt`: 포획 시간
    - `monsterId`: Monster 에 참조 가능한 몬스터 고유 ID 
    
### 스팟 (Spot)
- **필드**:
  - `_id`: 스팟의 고유 ID
  - `spotName`: 스팟의 이름
  - `dept`: 소속 단과대학
  - `description`: 스팟에 대한 설명
  - `lastPoint`: 가장 최근 점령될 때 사용된 포인트
  - `location`: 스팟의 위치 `(Object)`
    - `latitude`: 위도
    - `longitude`: 경도
  - `capturedBy`: 포획 여부 (`true` 또는 `false`)
---

## 참고 사항
- 모든 메시지는 UTF-8 인코딩된 JSON 문자열이어야 합니다.
- 서버와 클라이언트는 지속적인 연결을 유지해야 하며, 연결이 끊어질 경우 재연결을 시도해야 합니다.
- 클라이언트는 주기적으로 유저의 위치를 업데이트하여 근처 몬스터 정보를 최신 상태로 유지해야 합니다.

---


<!-- 
## 버전 정보
- **버전**: 1.0.0
- **최종 업데이트**: 2024-10-03

---

## 연락처
- **개발팀 이메일**:  -->
