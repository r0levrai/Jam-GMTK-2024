from flask import Flask, request, jsonify  # python -m pip install Flask

app = Flask(__name__)

drawings = []

@app.route('/draw-at-scale', methods=['POST'])
def receive_json():
    global drawings
    print(request.json)
    drawings.append(request.json)
    return jsonify({"message": "JSON received"}), 200

@app.route('/draw-at-scale/random', methods=['GET'])
def send_json():
    global drawings
    n = request.args.get('n', type=int)
    if n > 100:
        n = 100
    print(n, '->', {"drawings": drawings[-n:]})
    return jsonify({"drawings": drawings[-n:]}), 200

if __name__ == '__main__':
    app.run(debug=True)