import requests
import sys

arg = sys.argv

if len(arg) == 3:
    external_id = arg[1]
    status = arg[2]

    url = "http://localhost:8080/api/webhook/xendit"
    callback_token = "webhook_xendit_donasi"

    payload = {
        "external_id": external_id,
        "status": status
    }

    requests.post(url, json=payload, headers={"X-Callback-Token": callback_token})
