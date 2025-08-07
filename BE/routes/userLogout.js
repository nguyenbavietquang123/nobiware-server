import express from "express";
import { userLogout } from "../controllers/userLogout.js";

const router = express.Router();
router.post("/",userLogout);
export default router;