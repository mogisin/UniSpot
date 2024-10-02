const mongoose = require('mongoose');

const spotSchema = new mongoose.Schema({
  spotName: String,
  dept: String,
  description:String,
  lastPoint: String,
  
  location: {
    latitude: Number,
    longitude: Number,
  },
  
  capturedBy: String,
});

module.exports = mongoose.model('Spot', spotSchema);
