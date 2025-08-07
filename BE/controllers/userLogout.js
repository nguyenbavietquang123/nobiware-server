import axios from "axios"
import qs from "qs"
export const userLogout = async (req, res) => {
    const KEYCLOAK_BASE_URL = process.env.KEYCLOAK_BASE_URL;
    const FHIR_BASE_URL = process.env.FHIR_BASE_URL;
    const REALM = process.env.REALM;
    const { refresh_token } = req.body;
    const authHeader = req.headers['authorization'];


    try {
       const response = await axios.post(`${KEYCLOAK_BASE_URL}/realms/${REALM}/protocol/openid-connect/logout`, qs.stringify(req.body), {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            }
    });
        try {
            const token = qs.stringify({
                token: refresh_token
            })
            const blacklistResponse = await axios.post(`${FHIR_BASE_URL}/fhir/tokenblacklist`, token, {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                }
            });
            res.status(blacklistResponse.status).json({message: 'User logged out'})
        } catch (error) {
            console.error('Blacklist error:', error.blacklistResponse?.data || error.message);
            res.status(error.blacklistResponse?.status || 500).json({ error: 'Add refresh token to black list fail' });
        }    
    
    } catch (error) {
        console.error('Revoke error:', error.response?.data || error.message);
        res.status(error.response?.status || 500).json({ error: 'Token revocation failed' });
    }
}