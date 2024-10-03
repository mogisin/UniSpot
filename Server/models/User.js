const mongoose = require('mongoose');

const userSchema = new mongoose.Schema({
  username: String,
  money: {type:Number, default:0},
  location: {
    latitude: Number,
    longitude: Number,
  },
  monsters: [{
    name: String,
    capturedAt: Date,
    monsterId: { type: mongoose.Schema.Types.ObjectId, ref: 'Monster' }, // 몬스터의 ObjectId 참조 추가
    _id: false
  }
  ],
});

module.exports = mongoose.model('User', userSchema);
