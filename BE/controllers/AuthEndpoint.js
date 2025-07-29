
import { v4 as uuidv4 } from "uuid";

const authRequests = new Map()


export const getAuth = async (req, res) => {
    try {
        const KEYCLOAK_BASE_URL = process.env.KEYCLOAK_BASE_URL;
        const REALM = process.env.REALM;
        const PROXY_REDIRECT_URL= process.env.PROXY_REDIRECT_URL;
        const queryParams = req.query;
        const {
            response_type,
            client_id,
            redirect_uri,
            scope,
            state,
            aud,
            code_challenge,
            code_challenge_method
        } = queryParams;
        const role = "patient"
        const session_id = uuidv4();
        authRequests.set(session_id, {
            response_type,
            client_id,
            redirect_uri,
            scope,
            state,
            aud,
            code_challenge,
            code_challenge_method,
            role
        });
        console.log(KEYCLOAK_BASE_URL);
        console.log(REALM);
        const keycloakAuthUrl = new URL(`${KEYCLOAK_BASE_URL}/realms/${REALM}/protocol/openid-connect/auth`);
        keycloakAuthUrl.searchParams.set('response_type', response_type);
        keycloakAuthUrl.searchParams.set('client_id', client_id);
        keycloakAuthUrl.searchParams.set('redirect_uri', `${PROXY_REDIRECT_URL}/consent?session_id=${session_id}`);
        keycloakAuthUrl.searchParams.set('scope', "launch/patient openid fhirUser offline_access");
        keycloakAuthUrl.searchParams.set('code_challenge', "ouPWR4d-Xif13VAZ5mvjHSeFUOEPr7feSy3KsvaaMEk");
        keycloakAuthUrl.searchParams.set('code_challenge_method', code_challenge_method);
        console.log(queryParams);
        res.redirect(keycloakAuthUrl.toString());
    }
    catch (err) {
        console.error("getAuth failed:", err);
  res.status(500).json({ error: 'Internal server error' });
    }
}
export const postAuth = async (req,res)=> {
    try {
        const queryBody = req.body;
    const {
        response_type,
        client_id,
        redirect_uri,
        scope,
        state,
        aud,
        code_challenge,
        code_challenge_method
    } = queryBody;
    const role = "user";
    const session_id = uuidv4();
    authRequests.set(session_id,{
        response_type,
        client_id,
        redirect_uri,
        scope,
        state,
        aud,
        code_challenge,
        code_challenge_method,
        role
    })
    // const body = new URLSearchParams();
    // body.append('response_type',response_type);
    // body.append('client_id',client_id);
    // body.append('redirect_uri',redirect_uri);
    // body.append('scope',scope);
    // body.append('state',state);
    // body.append('aud',aud);
    // body.append('code_challenge',code_challenge);
    // body.append('code_challenge_method',code_challenge_method);
    // await fetch(`${KEYCLOAK_BASE_URL}/realms/${REALM}/protocol/openid-connect/auth`,{
    //     method: 'POST',
    //     headers: {'Content-Type':'application/x-www-form-urlencoded'},
    //     body: body,
    // });
     const keycloakAuthUrl = new URL(`${KEYCLOAK_BASE_URL}/realms/${REALM}/protocol/openid-connect/auth`);
        keycloakAuthUrl.searchParams.set('response_type', response_type);
        keycloakAuthUrl.searchParams.set('client_id', client_id);
        keycloakAuthUrl.searchParams.set('redirect_uri', `${PROXY_REDIRECT_URL}/consent?session_id=${session_id}&role=user`);
        keycloakAuthUrl.searchParams.set('scope', "launch/patient openid fhirUser offline_access");
        keycloakAuthUrl.searchParams.set('code_challenge', "ouPWR4d-Xif13VAZ5mvjHSeFUOEPr7feSy3KsvaaMEk");
        keycloakAuthUrl.searchParams.set('code_challenge_method', code_challenge_method);
        res.redirect(keycloakAuthUrl.toString());
    } catch (err) {
        console.error("getAuth failed:", err);
  res.status(500).json({ error: 'Internal server error' });
    }
    
}
export const getSessionInfo = async (req, res) =>{
const session = authRequests.get(req.query.session_id);
  if (!session) return res.status(404).send('Session not found');
  res.json(session); 

}