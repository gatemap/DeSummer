import tensorflow as tf
import numpy as np
import time
import cv2
import serial

arduino = serial.Serial('COM3', 9600)
time.sleep(1)

# Define the input size of the model
input_size = (224, 224)

# Open the web cam
cap = cv2.VideoCapture(0)  # webcam 불러오기
cap.set(cv2.CAP_PROP_BUFFERSIZE, 5)

# Load the saved model
model = tf.keras.models.load_model("keras_model.h5", compile=False)
while cap.isOpened():
    # Set time before model inference
    start_time = time.time()

    # Reading frames from the camera
    ret, frame = cap.read()
    if not ret:
        break

    # Resize the frame for the model
    model_frame = cv2.resize(frame, input_size, frame)
    # Expand Dimension (224, 224, 3) -> (1, 224, 224, 3) and Normalize the data
    model_frame = np.expand_dims(model_frame, axis=0) / 255.0

    # Predict
    is_normal_prob = model.predict(model_frame)[0]
    is_normal = np.argmax(is_normal_prob)

    # Compute the model inference time
    inference_time = time.time() - start_time
    fps = 1 / inference_time
    fps_msg = "Time: {:05.1f}ms {:.1f} FPS".format(inference_time * 1000, fps)

    # Add Information on screen
    if is_normal == 0:
        msg_normal = "normal"
        c = '1'
        c = c.encode('utf-8')
        arduino.write(c)
    elif is_normal == 1:
        msg_normal = "error"
        c = '0'
        c = c.encode('utf-8')
        arduino.write(c)
    else:
        msg_normal = "blank"

    msg_normal += " ({:.1f})%".format(is_normal_prob[is_normal] * 100)

    cv2.putText(frame, fps_msg, (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0, 0), thickness=1)
    cv2.putText(frame, msg_normal, (10, 70), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), thickness=2)

    # Show the result and frame
    cv2.imshow('product error detection', frame)

    # Press Q on keyboard to  exit
    if cv2.waitKey(25) & 0xFF == ord('q'):
        break