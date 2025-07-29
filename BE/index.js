import dotenv from 'dotenv';
dotenv.config();
import express from "express";
import cors from "cors";
import Auth from "./routes/authRoute.js";
const PORT = 5000;
const app = express();
app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

app.use("/auth", Auth);
app.listen(PORT, function () {
  console.log('Example app listening on port 5000!');
});
