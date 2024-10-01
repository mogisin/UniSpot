const mongoose = require('mongoose');

const userSchema = new mongoose.Schema({
  username: String,
  money: {type:Number, default:0},
  location: {
    latitude: Number,
    longitude: Number,
  },
  monsters: [{ name: String, capturedAt: Date }],
});

module.exports = mongoose.model('User', userSchema);
