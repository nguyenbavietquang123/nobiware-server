import express from "express";
import { getAuth,getSessionInfo, postAuth } from "../controllers/AuthEndpoint.js";

const router = express.Router();

router.get("/",getAuth);
router.get("/session",getSessionInfo);
router.post("/",postAuth);

export default router;