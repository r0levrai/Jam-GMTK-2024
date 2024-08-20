import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { ValidationPipe } from '@nestjs/common';
import * as fs from 'fs';
import { json, urlencoded } from 'body-parser';



async function bootstrap() {
  const httpsOptions = {
    key: fs.readFileSync('/home/khaled/ssl-certs/privkey.pem'),
    cert: fs.readFileSync('/home/khaled/ssl-certs/fullchain.pem'),
  };
  const app = await NestFactory.create(AppModule, { httpsOptions });
  app.useGlobalPipes(new ValidationPipe({ whitelist: true, forbidNonWhitelisted: true }));
  app.enableCors({
    origin: '*',
  });
  
  app.use(json({ limit: '50mb' }));
  app.use(urlencoded({ limit: '50mb', extended: true }));

  await app.listen(3000, '0.0.0.0');
}
bootstrap();
