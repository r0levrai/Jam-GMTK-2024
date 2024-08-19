import time

from flask import Flask, request, jsonify  # python -m pip install Flask


app = Flask(__name__)

drawings = []

@app.route('/drawings', methods=['POST'])
def receive_json():
    global drawings
    json = request.json
    json['time'] = time.time()
    print(json)
    drawings.append(json)
    return jsonify({"message": "JSON received"}), 200

@app.route('/draw-at-scale/lasts', methods=['GET'])
def send_json():
    global drawings
    n = request.args.get('n', default=1, type=int)
    if n > 100:
        n = 100
    print(n, '->', {"drawings": drawings[-n:]})
    return jsonify({"drawings": drawings[-n:]}), 200

if __name__ == '__main__':
    app.run(ssl_context=('ssl-certs/certificate.crt', 'ssl-certs/private.key'), host="0.0.0.0", port=55555, debug=True)