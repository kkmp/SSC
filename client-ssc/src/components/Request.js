import axios from "axios";

const request = async (params, callback = null, errorCallback = null, authorized = true) => {
    try {
        var config = {}
        var response = null;

        if (authorized) {
            const tokenRead = localStorage.getItem("token");
            if (tokenRead == null || tokenRead === '') {
                window.location = '/login'
            }
            config = {
                headers: {
                    'Authorization': 'Bearer ' + tokenRead,
                }
            }
        }

        const url = "https://localhost:7090" + params.url;
        switch (params.type) {
            case "GET":
                response = await axios.get(url, config);
                break
            case "POST":
                response = await axios.post(url, params.data, config);
                break
            case "PUT":
                response = await axios.put(url, params.data, config);
                break
            default:
                return
        }

        if (response.status === 200) {
            if (callback != null) {
                callback(response);
            }
        }
    } catch (e) {
        if (e.response.status === 401) {
            localStorage.removeItem("token");
            window.location = '/login'
        }
        else if (e.response.status >= 300 && e.response.status <= 500) {
            if (errorCallback != null) {
                errorCallback(e.response);
            }
        }
    }
}

export default request;