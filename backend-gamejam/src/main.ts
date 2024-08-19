import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { ValidationPipe } from '@nestjs/common';
import * as fs from 'fs';


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

  await app.listen(3000, '0.0.0.0');
}
bootstrap();
