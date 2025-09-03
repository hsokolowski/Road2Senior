import os, json, time
import pika, psycopg2

RABBIT_URL = os.getenv("RABBITMQ_URL", "amqp://guest:guest@rabbitmq:5672/")
QUEUE = os.getenv("RABBITMQ_QUEUE", "events")
PG = dict(host=os.getenv("POSTGRES_HOST","postgres"),
          dbname=os.getenv("POSTGRES_DB","r2s"),
          user=os.getenv("POSTGRES_USER","r2s"),
          password=os.getenv("POSTGRES_PASSWORD","r2s123"))

def ensure_table(cur):
    cur.execute("""CREATE TABLE IF NOT EXISTS events(
        id SERIAL PRIMARY KEY, payload JSONB NOT NULL, created_at TIMESTAMPTZ DEFAULT NOW()
    )""")

def handle(ch, method, props, body):
    payload = json.loads(body.decode())
    with psycopg2.connect(**PG) as conn, conn.cursor() as cur:
        ensure_table(cur)
        cur.execute("INSERT INTO events(payload) VALUES (%s)", [json.dumps(payload)])
    ch.basic_ack(method.delivery_tag)

def main():
    for _ in range(30):
        try:
            conn = pika.BlockingConnection(pika.URLParameters(RABBIT_URL)); break
        except: time.sleep(2)
    ch = conn.channel(); ch.queue_declare(queue=QUEUE, durable=True)
    ch.basic_qos(prefetch_count=1); ch.basic_consume(queue=QUEUE, on_message_callback=handle)
    print("Worker started."); ch.start_consuming()

if __name__ == "__main__": main()