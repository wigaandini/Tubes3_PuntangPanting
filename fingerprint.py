from PIL import Image
from fingerprint_sdk import FingerprintSensor

# Initialize the fingerprint sensor
sensor = FingerprintSensor()
sensor.connect()

# Capture fingerprint image
try:
    print('Waiting for finger...')

    while not sensor.isFingerPresent():
        pass

    img_data = sensor.captureImage()
    img = Image.frombytes('L', (256, 288), img_data)
    img.save('fingerprint.bmp')

    print('Fingerprint image saved as fingerprint.bmp')

except Exception as e:
    print('Error:', e)
finally:
    sensor.disconnect()
