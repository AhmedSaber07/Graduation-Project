from flask import Flask , request , render_template , url_for , jsonify 
from PIL import Image 
import numpy as np 
import tensorflow as tf 
import sys 
import os 
import cv2  
import io
from keras.models import load_model 
app = Flask(__name__) 
LABELS = ["cataract" , "diabetic_retinopathy" , "glaucoma" , "normal"] 
 
model = tf.keras.models.load_model('efficientnetb3-Eye Disease-93.36.h5') 
model.load_weights('efficientnetb3-Eye Disease-weights.h5') 
def prepare(image,img2): 
    photo = tf.keras.preprocessing.image.load_img(io.BytesIO(image.read()), target_size=(224, 224, 3))
    photo_tensor = tf.keras.preprocessing.image.img_to_array(photo) 
    photo_tensor = tf.expand_dims(photo_tensor, 0) 
    photo2 = tf.keras.preprocessing.image.load_img(io.BytesIO(img2.read()), target_size=(224, 224, 3))
    photo_tensor2 = tf.keras.preprocessing.image.img_to_array(photo2) 
    photo_tensor2 = tf.expand_dims(photo_tensor2, 0) 
 
 
    predictions = model.predict_generator(photo_tensor) 
    predictions2 = model.predict_generator(photo_tensor2) 
 
    highest_probability_index = np.argmax(predictions) 
    highest_probability_index2 = np.argmax(predictions2) 
 
    pred1 =[] 
    if highest_probability_index ==0: 
        pred1.append('cataracte') 
    elif highest_probability_index ==1: 
        pred1.append('diaberic_retinopathy')  
    elif highest_probability_index == 2: 
        pred1.append('glaucoma')  
    elif highest_probability_index==3: 
        pred1.append('normal')  
 
    if highest_probability_index2 ==0: 
        pred1.append('cataract')  
    elif highest_probability_index2 ==1: 
        pred1.append('diaberic_retinopathy')    
    elif highest_probability_index2 == 2: 
        pred1.append('glaucoma')   
    elif highest_probability_index2==3: 
        pred1.append('normal')  
    return pred1    
@app.route('/') 
def index(): 
    return render_template('index.html',appName="Image Classification") 
@app.route('/predictApi',methods=["POST"]) 
def api():  
    if 'fileup' not in request.files or 'fileup2' not in request.files: 
        return "please try again image doesn't exist" 
    image = request.files.get('fileup') 
    image2 = request.files.get('fileup2') 
    predication=prepare(image,image2) 
    return jsonify('prediction : '+  predication[0] + ' : ' +predication[1]) 
    
     
if __name__ =='__main__': 
    app.run(debug=True)