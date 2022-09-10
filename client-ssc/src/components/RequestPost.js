import axios from "axios";

const postRequest = async (url, data, callback, errorCallback = null, authorized = true) => {
    try {
        var response = null
        if (authorized) {
            const tokenRead = localStorage.getItem("token");
            if (tokenRead == null || tokenRead === '') {
                window.location = '/login'
            }
            const config = {
                headers: {
                    'Authorization': 'Bearer ' + tokenRead,
                }
            }
            response = await axios.post(url, data, config);
        } else {
            response = await axios.post(url, data);
        }

        if (response.status === 200) {
            callback(response);
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

export default postRequest;