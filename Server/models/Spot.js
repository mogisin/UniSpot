const mongoose = require('mongoose');

const spotSchema = new mongoose.Schema({
  spotName: String,
  dept: String,
  description:String,
  lastPoint: {type:Number, default:0},
  location: {
    latitude: Number,
    longitude: Number,
  },
  
  capturedBy: String,
});

module.exports = mongoose.model('Spot', spotSchema);
