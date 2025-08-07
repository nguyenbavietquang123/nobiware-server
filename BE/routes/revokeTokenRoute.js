import express from "express";
import { revokeToken } from "../controllers/revokeToken.js";

const router = express.Router();
router.post("/",revokeToken);
export default router;