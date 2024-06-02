from PIL import Image
import FingerprintSDK  # Assuming this is the SDK provided by your sensor manufacturer

# Initialize the fingerprint sensor
fingerprint_sensor = FingerprintSDK.FingerprintSensor()

if fingerprint_sensor.initialize():
    # Capture a fingerprint
    captured_fingerprint_data = fingerprint_sensor.captureFingerprint()

    if captured_fingerprint_data is not None:
        # Convert fingerprint data to an image
        fingerprint_image = Image.frombytes('L', (fingerprint_sensor.width, fingerprint_sensor.height),
                                            captured_fingerprint_data)

        # Save the fingerprint image
        fingerprint_image.save('fingerprint_image.png')
        print('Fingerprint image saved as fingerprint_image.png')

    else:
        print('Failed to capture fingerprint')

else:
    print('Failed to initialize fingerprint sensor')
