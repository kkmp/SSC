import getToken from "./GetToken";
import jwtDecode from "jwt-decode";

const getTokenData = () => {
    const tokenRead = getToken()

    if (tokenRead == null) {
        return null;
    }

    const decoded = jwtDecode(tokenRead);

    const id = decoded["nameid"]
    const name = decoded["name"]
    const surname = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"]
    const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]

    return {
        id: id,
        name: name,
        surname: surname,
        role: role
    }
}

export default getTokenData;