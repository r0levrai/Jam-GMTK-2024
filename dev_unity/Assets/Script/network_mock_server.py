import time, datetime

from flask import Flask, request, jsonify  # python -m pip install Flask
from flask_cors import CORS


app = Flask(__name__)
CORS(app, resources={r"/*": {"origins": "*"}})  #: "https://r0levrai.itch.io/draw-to-scale"}})  # Enable Cross-Origin Requests from itch.io web builds

drawings = []

@app.route('/', methods=['GET'])
def test():
    return "hello there :)", 200

@app.route('/drawings', methods=['POST'])
def receive_json():
    global drawings
    json = request.json
    json['createdDate'] = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%S.111Z") # = time.time()
    print(json)
    drawings.append(json)
    return jsonify({"message": "JSON received"}), 200

@app.route('/drawings/lasts', methods=['GET'])
def send_json():
    global drawings
    n = request.args.get('n', default=1, type=int)
    if n > 100:
        n = 100
    print(n, '->', {"drawings": drawings[-n:]})
    return jsonify({"drawings": drawings[-n:]}), 200

if __name__ == '__main__':
    app.run(ssl_context=('ssl-certs/fullchain.pem', 'ssl-certs/privkey.pem'), host="0.0.0.0", port=55555, debug=True)