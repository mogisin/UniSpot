const mongoose = require('mongoose');

const spotSchema = new mongoose.Schema({
  spotname: String,
  dept: String,
  description:String,

  location: {
    latitude: Number,
    longitude: Number,
  },
  
  capturedBy: String,
});

module.exports = mongoose.model('Spot', spotSchema);
