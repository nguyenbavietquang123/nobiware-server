import axios from "axios"
import qs from "qs"
import jwt from 'jsonwebtoken';
export const revokeToken = async (req, res) => {
    const KEYCLOAK_BASE_URL = process.env.KEYCLOAK_BASE_URL;
    const FHIR_BASE_URL = process.env.FHIR_BASE_URL;
    const REALM = process.env.REALM;
    const { token, token_type_hint } = req.body;
    const authHeader = req.headers['authorization'];

    if (!authHeader || !authHeader.startsWith('Basic ')) {
        return res.status(401).json({ error: 'Missing or invalid Authorization header' });
    }

    let data = "";
    if (token_type_hint == null) {
        data = qs.stringify({
            token: token
        })
    }
    else {
        data = qs.stringify({
            token: token,
            token_type_hint: token_type_hint
        })
    }
    try {
        const response = await axios.post(`${KEYCLOAK_BASE_URL}/realms/${REALM}/protocol/openid-connect/revoke`, data, {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'Authorization': authHeader
            }
        });
        try {
            const accessToken = qs.stringify({
                token: token
            })
            const blacklistResponse = await axios.post(`${FHIR_BASE_URL}/fhir/tokenblacklist`, accessToken, {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                }
            });
            const payload = jwt.decode(token);
            const typClaim = payload.typ;
            if (typClaim == "Refresh") {
                try {
                    const response = await axios.post(`${KEYCLOAK_BASE_URL}/realms/${REALM}/protocol/openid-connect/logout`, qs.stringify({refresh_token: token}), {
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'Authorization': authHeader
                        }
                    });
                } catch (error) {
                    console.error('Logout error:', error.blacklistResponse?.data || error.message);
                    res.status(error.blacklistResponse?.status || 500).json({ error: 'Token revocation failed' });
                }
            }

            res.status(blacklistResponse.status).json({ message: 'Token revoked' })
        } catch (error) {
            console.error('Blacklist error:', error.blacklistResponse?.data || error.message);
            res.status(error.blacklistResponse?.status || 500).json({ error: 'Token revocation failed' });
        }

    } catch (error) {
        console.error('Revoke error:', error.response?.data || error.message);
        res.status(error.response?.status || 500).json({ error: 'Token revocation failed' });
    }
}