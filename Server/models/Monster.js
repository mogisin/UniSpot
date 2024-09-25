const mongoose = require('mongoose');

const MonsterSchema = new mongoose.Schema({
  name: String,
  dept: String,
  description: String,  // 몬스터 설명
  tags: [String],  // 몬스터 태그 (배열 형태)
  
  location: {
    latitude: Number,
    longitude: Number,
  },
  captured: { type: Boolean, default: false },
});

module.exports = mongoose.model('Monster', MonsterSchema);
